using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace Assets.Code
{
    
    class RaycastMotionController : RaycastController, IMotionController
    {

        public LayerMask SolidLayer => solidLayer;
        public LayerMask JumpThroughLayer => oneWayLayer;
        public LayerMask AllLayer => solidLayer | oneWayLayer;

        public float maxClimbAngle = 140f;
        public float maxStepHeight = 0.2f;
        public float stepDistance = 0.2f;

        RaycastInfo info;

        public CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position)
        {
            return new CollisionInfo();
        }

        bool OnGround(out RaycastHit2D hit)
        {
            var bounds = boxCollider.bounds;
            bounds.Expand(-skinWidth);
            var ignore = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0, solidLayer);
            hit = RaycastLine(info.bottomLeft, info.bottomRight, Vector2.down, skinWidth * 2, ignore);
            return hit;
        }

        Vector2 GetMoveVector(Vector2 velocity, Vector2 normal)
        {
            var cw = Utils.Clockwise(normal);
            var ccw = Utils.CounterClockwise(normal);
            if (Vector3.Dot(velocity, cw) == 0) return Vector3.zero;
            return (Vector3.Dot(velocity, cw) >= 0 ? cw : ccw).normalized * velocity.magnitude;
        }

        void MoveX(float x)
        {
            if (x == 0) return;
            var pos = transform.position;
            var downHit = RaycastLine(info.bottomLeft, info.bottomRight, Vector2.down, skinWidth * 2, null);
            var hit = RaycastLine(x < 0 ? info.topLeft : info.topRight, x < 0 ? info.bottomLeft : info.bottomRight, Vector2.right * (x > 0 ? 1 : -1), Mathf.Abs(x) + skinWidth, null);
            if (hit && !downHit)
            {
                var distance = Mathf.Max(hit.distance - skinWidth);
                pos.x += Mathf.Sign(x) * distance;
                transform.position = pos;
                Recalculate();
                var rem = Mathf.Sign(x) * (Mathf.Abs(x) - distance);
                if (CanStand(hit.normal))
                {
                    var move = GetMoveVector(new Vector2(rem, 0), hit.normal);
                    Debug.DrawRay(hit.point, move.normalized * .3f, Color.blue);
                    var topHit = RaycastLine(info.topLeft, info.topRight, move, Mathf.Max(0, Mathf.Abs(x) - distance), null);
                    var sideHit = RaycastLine(x > 0 ? info.topRight : info.topLeft, x > 0 ? info.bottomRight : info.bottomLeft, move, Mathf.Max(0, Mathf.Abs(x) - distance), null);
                    if (!topHit && !sideHit)
                    {
                        pos += (Vector3)move.normalized * Mathf.Max(0, move.magnitude - skinWidth);
                    }
                    else
                    {
                        var shortestHit = !sideHit ? topHit : (!topHit ? sideHit : (topHit.distance < sideHit.distance ? topHit : sideHit));
                        pos += (Vector3)move.normalized * (Mathf.Max(0, Mathf.Min(Mathf.Abs(x) - distance - skinWidth, shortestHit.distance - skinWidth)));
                    }
                }
                if (x < 0)
                {
                    info.collision.Left = true;
                }
                else
                {
                    info.collision.Right = true;
                }
            }
            else if (hit)
            {
                var distance = Mathf.Max(hit.distance - skinWidth);
                pos.x += Mathf.Sign(x) * distance;
                transform.position = pos;
                Recalculate();
                var rem = Mathf.Sign(x) * (Mathf.Abs(x) - distance);
                if (CanStand(hit.normal))
                {
                    var move = GetMoveVector(new Vector2(rem, 0), hit.normal);
                    Debug.DrawRay(hit.point, move.normalized * .3f, Color.blue);
                    var topHit = RaycastLine(move.y > 0 ? info.topLeft : info.bottomLeft, move.y > 0 ? info.topRight : info.bottomRight, move, Mathf.Max(0, Mathf.Abs(x) - distance), null);
                    var sideHit = RaycastLine(x > 0 ? info.topRight : info.topLeft, x > 0 ? info.bottomRight : info.bottomLeft, move, Mathf.Max(0, Mathf.Abs(x) - distance), null);
                    if (!topHit && !sideHit)
                    {
                        pos += (Vector3)move.normalized * Mathf.Max(0, move.magnitude - skinWidth);
                    }
                    else
                    {
                        var shortestHit = !sideHit ? topHit : (!topHit ? sideHit : (topHit.distance < sideHit.distance ? topHit : sideHit));
                        pos += (Vector3)move.normalized * (Mathf.Max(0, Mathf.Min(Mathf.Abs(x) - distance - skinWidth, shortestHit.distance - skinWidth)));
                    }
                }
                if (x < 0)
                {
                    info.collision.Left = true;
                }
                else
                {
                    info.collision.Right = true;
                }
            }
            else
            {
                pos.x += x;
            }
            transform.position = pos;
            Recalculate();
        }

        void MoveY(float y)
        {
            if (y == 0) return;
            var pos = transform.position;
            var hit = RaycastLine(y > 0 ? info.topLeft : info.bottomLeft, y > 0 ? info.topRight : info.bottomRight, Mathf.Sign(y) * Vector2.up, Mathf.Abs(y) + skinWidth, null);
            if (hit)
            {
                var distance = Mathf.Max(hit.distance - skinWidth);
                pos.y += Mathf.Sign(y) * distance;
                if (y < 0 && CanStand(hit.normal))
                {
                    info.collision.Below = true;
                }
                else if (y < 0)
                {
                    info.collision.Below = false;
                    var move = GetMoveVector(new Vector2(0, -(Mathf.Abs(y) - distance)), hit.normal);
                    hit = RaycastLine(info.bottomLeft, info.bottomRight, move, Mathf.Abs(y) - distance, null);
                    if (!hit)
                    {
                        pos += (Vector3)move.normalized * Mathf.Max(0, move.magnitude - skinWidth);
                        transform.position = pos;
                        Recalculate();
                        return;
                    }
                    else
                    {
                        pos += (Vector3)move.normalized * (Mathf.Max(0, Mathf.Abs(y) - distance - skinWidth));
                        transform.position = pos;
                        Recalculate();
                        return;
                    }
                }
                else
                {
                    info.collision.Above = true;
                }
            }
            else
            {
                pos.y += y;
            }
            transform.position = pos;
            Recalculate();
        }

        public bool CanStand(Vector2 normal)
        {
            var a = Vector2.Angle(Vector2.up, normal);
            if (a < 180)
            {
                return a < 65;
            }
            return 360 - a < 65;
        }

        void Recalculate()
        {
            info.topLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.max.y);
            info.topRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.max.y);
            info.bottomLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y);
            info.bottomRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            info = new RaycastInfo();
            info.collision = new CollisionInfo();
            Recalculate();
            MoveX(velocity.x);
            MoveY(velocity.y);
            return info.collision;
        }

        public bool OnJumpThrough(Vector3 down)
        {
            return false;
        }
    }

    class RaycastInfo
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
        public CollisionInfo collision = new CollisionInfo();
    }


}