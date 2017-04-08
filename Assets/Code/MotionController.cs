using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(BoxCollider2D))]
    class MotionController : RaycastController
    {

        public float maxClimbAngle = 140f;
        public float maxStepHeight = 0.1f;
        public float stepDistance = 0.1f;

        Vector3 GetMoveVector(Vector3 normal, Vector3 velocity)
        {
            var g1 = new Vector3(-normal.y, normal.x);
            var g2 = g1 * -1;
            if (Vector3.Dot(velocity, g1) == 0) return Vector3.zero;
            return (Vector3.Dot(velocity, g1) >= 0 ? g1 : g2).normalized * velocity.magnitude;
        }

        float ClampAngle(float angle)
        {
            if (angle <= 180) return angle;
            return 360 - angle;
        }

        float ReflectAngle(float angle)
        {
            if (angle < 90)
            {
                return angle - (angle - 90) * 2;
            }
            return angle + (90 - angle) * 2;
        }

        Vector3 Clockwise(Vector3 input)
        {
            return new Vector3(-input.y, input.x);
        }

        Vector3 CounterClockwise(Vector3 input)
        {
            return Clockwise(input) * -1;
        }

        bool StepUp(Vector3 velocity, Vector3 down, Vector3 original, out CollisionInfo info)
        {
            info = new CollisionInfo();
            var dx = GetMoveVector(down, velocity.normalized) * stepDistance;
            var dy = down * -maxStepHeight;
            var bounds = boxCollider.bounds;
            bounds.center = transform.position + dx + dy;
            var hit = BoxCast(bounds, down, maxStepHeight * 2, solidLayer);
            if (hit && hit.distance > skinWidth) // hit should never be false. hit.distance needs to be greater than skinWidth or else you're casting from inside an object
            {
                if (ClampAngle(Vector3.Angle(down, GetMoveVector(hit.normal, down))) < maxClimbAngle && Vector3.Dot(down, hit.normal) < 0) // jump-through platform is standable
                {
                    info.Below = true;
                    transform.position += dx + dy + (down * Mathf.Max(hit.distance - skinWidth, 0));
                    return true;
                }
            }
            return false;
        }

        CollisionInfo Travel(Vector3 velocity, Vector3 down, Vector3 original)
        {
            var oneWayHit = BoxCast(boxCollider.bounds, velocity, velocity.magnitude, oneWayLayer);
            var rem = velocity;
            var hit = BoxCast(boxCollider.bounds, rem, rem.magnitude, solidLayer);

            if (oneWayHit && (!hit || hit.distance > oneWayHit.distance) && Vector3.Dot(down, original) >= 0 && oneWayHit.distance > skinWidth)
            {
                var travel = rem.normalized * (Mathf.Max(oneWayHit.distance - skinWidth, 0));
                transform.position += travel;
                rem -= travel;
                var rot = GetMoveVector(oneWayHit.normal, rem.normalized);
                if (ClampAngle(Vector3.Angle(down, rot)) < maxClimbAngle && Vector3.Dot(down, oneWayHit.normal) < 0) // jump-through platform is standable
                {
                    if (rem.magnitude < skinWidth) return new CollisionInfo();
                    var info = new CollisionInfo { Below = true };
                    return info.Or(Move_Impl(rot * Vector3.Dot(rem, rot), down, original));
                }
            }

            hit = BoxCast(boxCollider.bounds, rem, rem.magnitude, solidLayer);
            if (hit)
            {
                var travel = velocity.normalized * (Mathf.Max(hit.distance - skinWidth, 0));
                transform.position += travel;
                rem -= travel;
                var rot = GetMoveVector(hit.normal, rem.normalized);
                if (Vector3.Distance(velocity.normalized, down.normalized) < skinWidth) // moving directly downwards
                {
                    if (ClampAngle(Vector3.Angle(down, rot)) >= ReflectAngle(maxClimbAngle)) // standing on slope steeper than max climb angle
                    {
                        return new CollisionInfo();
                    }
                }
                else if (ClampAngle(Vector3.Angle(down, rot)) > maxClimbAngle && Vector3.Dot(down, original) >= 0) // trying to move up a wall while originally not moving upwards
                {
                    if (StepUp(velocity, down, original, out var info))
                    {
                        return info;
                    }
                    return new CollisionInfo()
                    {
                        Left = Vector3.Dot(CounterClockwise(down), original) > 0,
                        Right = Vector3.Dot(Clockwise(down), original) > 0
                    };
                }
                if (rem.magnitude < skinWidth) return new CollisionInfo();
                return Move_Impl(rot * Vector3.Dot(rem, rot), down, original);
            }
            transform.position += velocity;
            return new CollisionInfo();
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            return Move_Impl(velocity, down.normalized, velocity);
        }

        CollisionInfo Move_Impl(Vector3 velocity, Vector3 down, Vector3 original)
        {
            RaycastHit2D hit;
            var collider = BoxCheck(boxCollider.bounds, oneWayLayer);
            var layer = collider ? solidLayer : (LayerMask) (oneWayLayer | solidLayer);
            hit = BoxCast(boxCollider.bounds, down, skinWidth, layer);

            if (hit && Vector3.Dot(velocity, down) > 0)
            {
                var info = new CollisionInfo { Below = true };
                var move = GetMoveVector(hit.normal, velocity);

                if (ClampAngle(Vector3.Angle(down, move)) > maxClimbAngle) return info;

                if (Vector3.Distance(velocity.normalized, down.normalized) < skinWidth) // we are moving directly downwards
                {
                    if (ClampAngle(Vector3.Angle(down, move)) >= ReflectAngle(maxClimbAngle)) // we aren't standing on a steep slope, slide down
                    {
                        transform.position += velocity.normalized * (hit.distance - skinWidth);
                        return info;
                    }
                    else
                    {
                        info.Below = false;
                    }
                }
                return info.Or(Travel(move, down, original));
            }
            return Travel(velocity, down, original);
        }

    }

    struct CollisionInfo
    {
        public bool Below { get; set; }
        public bool Above { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public CollisionInfo Or(CollisionInfo info)
        {
            return new CollisionInfo()
            {
                Below = info.Below || Below,
                Above = info.Above || Above,
                Left = info.Left || Left,
                Right = info.Right || Right,
            };
        }
    }

}
