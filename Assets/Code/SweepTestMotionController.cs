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
            return false;
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
                var vel = velocity.normalized * (hit.distance - skinWidth);
                var rem = GetMoveVector(velocity.normalized * (velocity.magnitude - hit.distance), hit.normal);
                var mov = rem + vel;
                if (!body.SweepTest(rem + vel, out hit, mov.magnitude + skinWidth))
                {
                    body.MovePosition(transform.position + mov);
                }
                else
                {
                    body.MovePosition(transform.position + mov.normalized * (hit.distance - skinWidth));
                }
                //body.MovePosition(transform.position + velocity.normalized * (hit.distance - skinWidth));

                if (Vector3.Dot(velocity, down) > 0)
                {
                    info.Below = true;
                }
            }
            return info;
        }

        public bool OnJumpThrough(Vector3 down, out Collider collider)
        {
            collider = null;
            return false;
        }
    }
}
