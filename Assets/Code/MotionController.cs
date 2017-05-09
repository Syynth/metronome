﻿using System;
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
        public float maxStepHeight = 0.15f;
        public float stepDistance = 0.15f;

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

        public Vector3 Clockwise(Vector3 input)
        {
            return new Vector3(-input.y, input.x);
        }

        public Vector3 CounterClockwise(Vector3 input)
        {
            return Clockwise(input) * -1;
        }

        bool StepUp(Vector3 velocity, Vector3 down, Vector3 original, out CollisionInfo info, ref Vector3 position, ref Bounds bounds)
        {
            info = new CollisionInfo();
            var dx = GetMoveVector(down, velocity.normalized) * stepDistance;
            var dy = down * -maxStepHeight;
            var newBounds = bounds;
            newBounds.center = position + dx + dy;
            var hit = BoxCast(newBounds, down, maxStepHeight * 2, solidLayer);
            if (hit && hit.distance > 0) // hit should never be false. hit.distance needs to be greater than skinWidth or else you're casting from inside an object
            {
                if (ClampAngle(Vector3.Angle(down, GetMoveVector(hit.normal, down))) < maxClimbAngle && Vector3.Dot(down, hit.normal) < 0) // jump-through platform is standable
                {
                    info.Below = true;
                    var travel = dx + dy; // + (down * Mathf.Max(hit.distance - skinWidth, 0));
                    position += travel;
                    bounds.center += travel;
                    info = Travel(down, down, down, ref position, ref bounds);
                    return true;
                }
            }
            return false;
        }

        CollisionInfo Travel(Vector3 velocity, Vector3 down, Vector3 original, ref Vector3 position, ref Bounds bounds)
        {
            var oneWayHit = BoxCast(bounds, velocity, velocity.magnitude, oneWayLayer);
            var rem = velocity;
            var hit = BoxCast(bounds, rem, rem.magnitude, solidLayer);

            if (oneWayHit && (!hit || hit.distance > oneWayHit.distance) && Vector3.Dot(down, original) >= 0 && oneWayHit.distance > skinWidth)
            {
                var travel = rem.normalized * (Mathf.Max(oneWayHit.distance - skinWidth, 0));
                position += travel;
                bounds.center += travel;
                rem -= travel;
                var rot = GetMoveVector(oneWayHit.normal, rem.normalized);
                if (ClampAngle(Vector3.Angle(down, rot)) < maxClimbAngle && Vector3.Dot(down, oneWayHit.normal) < 0) // jump-through platform is standable
                {
                    if (rem.magnitude < skinWidth) return new CollisionInfo();
                    var info = new CollisionInfo { Below = true };
                    return info.Or(Move_Impl(rot * Vector3.Dot(rem, rot), down, original, ref position, ref bounds));
                }
            }

            hit = BoxCast(bounds, rem, rem.magnitude, solidLayer);
            if (hit)
            {
                var travel = velocity.normalized * (Mathf.Max(hit.distance - skinWidth, 0));
                position += travel;
                bounds.center += travel;
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
                    if (StepUp(velocity, down, original, out var info, ref position, ref bounds))
                    {
                        return info;
                    }
                    return new CollisionInfo()
                    {
                        Left = Vector3.Dot(CounterClockwise(down), original) > 0,
                        Right = Vector3.Dot(Clockwise(down), original) > 0
                    };
                }
                //if (rem.magnitude < skinWidth) return new CollisionInfo();
                //return Move_Impl(rot * Vector3.Dot(rem, rot), down, original, ref position, ref bounds);
                var go = rot * Vector3.Dot(rem, rot);
                hit = BoxCast(bounds, go, go.magnitude + skinWidth, solidLayer);
                if (hit)
                {
                    travel = go.normalized * (Mathf.Max(hit.distance - skinWidth, 0));
                    position += travel;
                    bounds.center += travel;
                    return new CollisionInfo();
                }
                position += go;
                bounds.center += go;
                return new CollisionInfo();
            }
            position += velocity;
            bounds.center += velocity;
            return new CollisionInfo();
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            var position = transform.position;
            var bounds = boxCollider.bounds;
            var before = Physics2D.OverlapBox(position, bounds.size, 0, solidLayer);
            var info = Move_Impl(velocity, down.normalized, velocity, ref position, ref bounds);
            var after = Physics2D.OverlapBox(position, bounds.size, 0, solidLayer);
            if (before == null && after != null)
            {
                Debug.Break();
            }
            position = transform.position;
            bounds = boxCollider.bounds;
            info = Move_Impl(velocity, down.normalized, velocity, ref position, ref bounds);
            transform.position = position;
            return info;
        }

        CollisionInfo Move_Impl(Vector3 velocity, Vector3 down, Vector3 original, ref Vector3 position, ref Bounds bounds)
        {
            RaycastHit2D hit;
            var oneWayHit = BoxCast(bounds, down, skinWidth, oneWayLayer);
            var layer = oneWayHit && oneWayHit.distance == 0 ? solidLayer : (LayerMask) (oneWayLayer | solidLayer);
            hit = BoxCast(bounds, down, skinWidth, layer);

            if (hit && Vector3.Dot(original, down) > 0) // we've hit something and we're originally travelling downwards
            {
                var info = new CollisionInfo { Below = true }; // default case, we've hit something below us
                var move = GetMoveVector(hit.normal, velocity); // get vector along ground in direction of velocity

                if (ClampAngle(Vector3.Angle(down, move)) > maxClimbAngle) return new CollisionInfo(); // new direction is too steep, return that we aren't on ground

                if (ClampAngle(Vector3.Angle(down, move)) < ReflectAngle(maxClimbAngle)) // we are standing on a steep slope, doesn't count as ground
                {
                    info.Below = false;
                }
                else if (Vector3.Distance(original.normalized, down.normalized) < skinWidth) // we are moving directly downwards
                {
                    if (ClampAngle(Vector3.Angle(down, move)) >= ReflectAngle(maxClimbAngle)) // we are standing on a steep slope, slide down
                    {
                        var travel = down.normalized * (Mathf.Max(hit.distance - skinWidth, 0));
                        position += travel;
                        bounds.center += travel;
                        return info;
                    }
                }
                return info.Or(Travel(move, down, original, ref position, ref bounds));
            }
            return Travel(velocity, down, original, ref position, ref bounds);
        }

        public bool OnJumpThrough(Vector3 down)
        {
            var oneWayHit = BoxCast(boxCollider.bounds, down, skinWidth, oneWayLayer);
            var hit = BoxCast(boxCollider.bounds, down, skinWidth * 2, solidLayer);
            return (!hit && oneWayHit && oneWayHit.distance > 0);
        }

        public CollisionInfo CheckMove(Vector3 direction, Vector3 down, Vector3 position)
        {
            var bounds = boxCollider.bounds;
            return Move_Impl(direction, down.normalized, direction, ref position, ref bounds);
        }

    }

    public struct CollisionInfo
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
