using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    class SweepTestMotionController : MonoBehaviour, IMotionController
    {

        public LayerMask solidLayer;
        public LayerMask oneWayLayer;

        public LayerMask SolidLayer => solidLayer;

        public LayerMask JumpThroughLayer => oneWayLayer;

        public LayerMask AllLayer => solidLayer | oneWayLayer;

        public const float skinWidth = 0.02f;

        Vector3 GetMoveVector(Vector3 velocity, Vector3 normal)
        {
            var cw = Utils.Clockwise(normal);
            var ccw = Utils.CounterClockwise(normal);
            if (Vector3.Dot(velocity, cw) == 0) return Vector3.zero;
            return (Vector3.Dot(velocity, cw) >= 0 ? cw : ccw).normalized * velocity.magnitude;
        }

        public bool CanStand(Vector3 normal)
        {
            var a = Vector3.Angle(Vector3.up, normal);
            if (a < 180)
            {
                return a < 65;
            }
            return 360 - a < 65;
        }

        public CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position)
        {
            return new CollisionInfo();
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down, List<Collider> ignore)
        {
            var body = GetComponent<Rigidbody>();
            RaycastHit hit;
            CollisionInfo info = new CollisionInfo();
            if (!body.SweepTest(velocity, out hit, velocity.magnitude + skinWidth))
            {
                body.MovePosition(transform.position + velocity);
            }
            else
            {
                if (Vector3.Dot(velocity, down) > 0)
                {
                    info.Below = true;
                }
                var vel = velocity.normalized * (hit.distance - skinWidth);
                if (Vector3.Distance(velocity.normalized, down.normalized) < skinWidth)
                {
                    body.MovePosition(transform.position + vel);
                    return info;
                }
                var rem = GetMoveVector(velocity.normalized * (velocity.magnitude - hit.distance), hit.normal);
                var mov = rem + vel;
                if (!body.SweepTest(rem + vel, out hit, mov.magnitude + skinWidth))
                {
                    body.MovePosition(transform.position + mov);
                }
                else
                {
                    body.MovePosition(transform.position + mov.normalized * (hit.distance - skinWidth));
                    if (Vector3.Dot(mov, down) > 0)
                    {
                        info.Below = true;
                    }
                }

            }
            return info;
        }

        public bool OnJumpThrough(Vector3 down, out Collider collider)
        {
            RaycastHit hit;
            if (GetComponent<Rigidbody>().SweepTest(down, out hit, skinWidth * 2))
            {
                collider = hit.collider;
                return true;
            }
            collider = null;
            return false;
        }
    }
}
