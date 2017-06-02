using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(BoxCollider))]
    class BoxcastMotionController : RaycastController, IMotionController
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

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.separation >= 0) continue;
                var move = contact.separation * contact.normal;
                transform.position += move;
            }
        }

        bool StepUp(Vector3 velocity, Vector3 down, Vector3 original, out CollisionInfo info, ref Vector3 position, ref Bounds bounds)
        {
            info = new CollisionInfo();
            var dx = GetMoveVector(down, velocity.normalized) * stepDistance;
            var dy = down * -maxStepHeight;
            var newBounds = bounds;
            newBounds.center = position + dx + dy;
            var hit = BoxCast(newBounds, down, maxStepHeight * 2, solidLayer);
            if (hit.IsValid() && hit.distance > 0) // hit should never be false. hit.distance needs to be greater than skinWidth or else you're casting from inside an object
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

            if (oneWayHit.IsValid() && (!hit.IsValid() || hit.distance > oneWayHit.distance) && Vector3.Dot(down, original) >= 0 && oneWayHit.distance > skinWidth)
            {
                var travel = rem.normalized * (Mathf.Max(oneWayHit.distance - skinWidth, 0));
                position += travel;
                bounds.center += travel;
                rem -= travel;
                var rot = GetMoveVector(oneWayHit.normal, rem.normalized);
                if (Vector3.Dot(rem.normalized, rot.normalized) > 0.95)
                {
                    rot = hit.normal;
                    rot.Normalize();
                }
                if (ClampAngle(Vector3.Angle(down, rot)) < maxClimbAngle && Vector3.Dot(down, oneWayHit.normal) < 0) // jump-through platform is standable
                {
                    if (rem.magnitude < skinWidth) return new CollisionInfo();
                    var info = new CollisionInfo { Below = true };
                    return info.Or(Move_Impl(rot * Vector3.Dot(rem, rot), down, original, ref position, ref bounds));
                }
            }

            hit = BoxCast(bounds, rem, rem.magnitude, solidLayer);
            if (hit.IsValid())
            {
                int count = 0;
                while (hit.IsValid() && hit.distance == 0 && count < 10)
                {
                    count++;
                    print("distance was zero");

                    var move = 2 * skinWidth * Vector3.up;
                    bounds.center += move;
                    transform.position += move;
                    hit = BoxCast(bounds, down, velocity.magnitude, solidLayer);
                }

                var travel = velocity.normalized * (Mathf.Max(hit.distance - skinWidth, 0));
                position += travel;
                bounds.center += travel;
                rem -= travel;
                var rot = GetMoveVector(hit.normal, rem.normalized);
                if (Vector3.Dot(rem.normalized, rot.normalized) > 0.95)
                {
                    rot += hit.normal;
                    rot.Normalize();
                }
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
                if (rem.magnitude < skinWidth) return new CollisionInfo();
                return Move_Impl(rot * Vector3.Dot(rem, rot), down, original, ref position, ref bounds);
            }
            position += velocity;
            bounds.center += velocity;
            return new CollisionInfo();
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            if (Input.GetButtonDown("Debug"))
            {
                Debug.Break();
            }
            var position = transform.position;
            var bounds = boxCollider.bounds;
            var info = Move_Impl(velocity, down.normalized, velocity, ref position, ref bounds);
            transform.position = position;
            return info;
        }

        CollisionInfo Move_Impl(Vector3 velocity, Vector3 down, Vector3 original, ref Vector3 position, ref Bounds bounds)
        {
            RaycastHit hit;
            var bnd = bounds;
            bnd.Expand(-skinWidth);
            var oneWayHit = BoxCast(bnd, down, skinWidth, oneWayLayer);
            var layer = oneWayHit.IsValid() && oneWayHit.distance == 0 ? solidLayer : (LayerMask) (oneWayLayer | solidLayer);
            hit = BoxCast(bounds, down, skinWidth, layer);

            int count = 0;
            while (hit.IsValid() && hit.distance == 0 && count < 10)
            {
                count++;
                print("distance was zero 2");

                Physics.ComputePenetration(boxCollider, bounds.center, Quaternion.identity, hit.collider, hit.collider.transform.position, hit.collider.transform.rotation, out var direction, out var distance);
                var move = direction * distance;
                bounds.center += move;
                transform.position += move;
                hit = BoxCast(bounds, down, velocity.magnitude, layer);
            }
            if (hit.IsValid() && Vector3.Dot(velocity, down) > 0)
            {
                var info = new CollisionInfo { Below = true };
                var move = GetMoveVector(hit.normal, velocity);

                if (ClampAngle(Vector3.Angle(down, move)) > maxClimbAngle) return new CollisionInfo();

                if (ClampAngle(Vector3.Angle(down, move)) < ReflectAngle(maxClimbAngle))
                {
                    info.Below = false;
                }
                else if (Vector3.Distance(velocity.normalized, down.normalized) < skinWidth) // we are moving directly downwards
                {
                    if (ClampAngle(Vector3.Angle(down, move)) >= ReflectAngle(maxClimbAngle)) // we aren't standing on a steep slope, slide down
                    {
                        var travel = down * Mathf.Max(hit.distance - skinWidth, 0);
                        position += travel;
                        bounds.center += travel;
                        return info;
                    }
                }
                CollisionInfo returnValue = info;
                try
                {
                    returnValue = info.Or(Travel(move, down, original, ref position, ref bounds));
                }
                catch (StackOverflowException ex)
                {
                    print(hit.distance);
                }
                return returnValue;
            }
            CollisionInfo rv = new CollisionInfo();
            try
            {
                rv = Travel(velocity, down, original, ref position, ref bounds);
            }
            catch (StackOverflowException ex)
            {
                print(string.Format("Velocity: {0}, hit: {1}, hit.distance: {2}", velocity, hit.IsValid(), hit.distance));
                Debug.Break();
            }
            return rv;
        }

        public bool OnJumpThrough(Vector3 down)
        {
            var oneWayHit = BoxCast(boxCollider.bounds, down, skinWidth, oneWayLayer);
            var hit = BoxCast(boxCollider.bounds, down, skinWidth * 2, solidLayer);
            return (!hit.IsValid() && oneWayHit.IsValid() && oneWayHit.distance > 0);
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
