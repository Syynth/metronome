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
            var hit = RaycastLine(x < 0 ? info.topLeft : info.topRight, x < 0 ? info.bottomLeft : info.bottomRight, Vector2.right * (x > 0 ? 1 : -1), Mathf.Abs(x) + skinWidth, null);
            if (hit)
            {
                pos.x += Mathf.Sign(x) * Mathf.Max(hit.distance - skinWidth * 2, 0);
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
        }

        void MoveY(float y)
        {
            if (y == 0) return;
            var pos = transform.position;
            var hit = RaycastLine(y > 0 ? info.topLeft : info.bottomLeft, y > 0 ? info.topRight : info.bottomRight, Mathf.Sign(y) * Vector2.up, Mathf.Abs(y) + skinWidth, null);
            if (hit)
            {
                pos.y += Mathf.Sign(y) * Mathf.Max(hit.distance - skinWidth * 2, 0);
                if (y < 0)
                {
                    info.collision.Below = true;
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
        }

        void Recalculate()
        {
            info.collision = new CollisionInfo();
            info.topLeft = new Vector2(boxCollider.bounds.min.x + skinWidth, boxCollider.bounds.max.y - skinWidth);
            info.topRight = new Vector2(boxCollider.bounds.max.x - skinWidth, boxCollider.bounds.max.y - skinWidth);
            info.bottomLeft = new Vector2(boxCollider.bounds.min.x + skinWidth, boxCollider.bounds.min.y + skinWidth);
            info.bottomRight = new Vector2(boxCollider.bounds.max.x - skinWidth, boxCollider.bounds.min.y + skinWidth);
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            info = new RaycastInfo();
            Recalculate();
            if (Vector2.Dot(velocity, Vector2.down) > 0 && OnGround(out var hit))
            {
                info.collision.Below = true;
                var move = GetMoveVector(velocity, hit.normal);
                if (move.y > 0)
                {
                    MoveY(move.y);
                    MoveX(move.x);
                }
                else
                {
                    MoveX(move.x);
                    MoveY(move.y);
                }
            }
            else
            {
                MoveX(velocity.x);
                MoveY(velocity.y);
            }
            return info.collision;
        }

        public bool OnJumpThrough(Vector3 down)
        {
            return false;
        }
    }

    struct RaycastInfo
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
        public CollisionInfo collision;
    }


}