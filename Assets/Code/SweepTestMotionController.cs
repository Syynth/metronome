using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        [Range(1, 89)]
        public float maxClimbAngle = 65f;

        Rigidbody body;

        void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        bool SweepTest(Vector3 direction, out RaycastHit raycastHit, float maxDistance, List<Collider> ignore)
        {
            raycastHit = body.SweepTestAll(direction, maxDistance).Where(h => !ignore.Contains(h.collider)).OrderBy(h => h.distance).FirstOrDefault();
            return raycastHit.collider != null;
        }

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
                return a < maxClimbAngle;
            }
            return 360 - a < maxClimbAngle;
        }

        bool CanMove(Vector3 velocity)
        {
            var a = Vector3.Angle(Vector3.right, new Vector3(Mathf.Abs(velocity.x), velocity.y));
            if (a < 180)
            {
                return a < maxClimbAngle;
            }
            return 360 - a < maxClimbAngle;
        }

        public CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position)
        {
            return new CollisionInfo();
        }

        RaycastHit MoveTo(Vector3 velocity, List<Collider> ignore, out Vector3 vel)
        {
            RaycastHit hit;
            if (!SweepTest(velocity, out hit, velocity.magnitude + skinWidth, ignore))
            {
                body.MovePosition(transform.position + velocity);
                vel = velocity;
            }
            else
            {
                vel = velocity.normalized * (hit.distance - skinWidth);
            }
            return hit;
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down, List<Collider> ignore, bool findGround)
        {
            CollisionInfo info = new CollisionInfo();
            var isUp = Vector3.Dot(velocity, down) < 0;
            var isDown = Vector3.Dot(velocity, down) > 0;
            var straightDown = Vector3.Distance(velocity.normalized, down.normalized) < skinWidth;
            Vector3 downVel;
            var downHit = MoveTo(down.normalized * velocity.magnitude, ignore, out downVel);
            Vector3 vel1;
            var hit1 = MoveTo(velocity, ignore, out vel1);
            var rem1 = velocity - vel1;
            if (hit1.collider == null)
            {
                if (isDown && downHit.collider != null && findGround && CanStand(downHit.normal))
                {
                    info.Below = true;
                    Vector3 vel2;
                    Vector3 vel = GetMoveVector(velocity, downHit.normal);
                    var mov = vel + downVel;
                    var hit2 = MoveTo(mov, ignore, out vel2);
                    body.MovePosition(transform.position + vel2);
                    return info;
                }
                body.MovePosition(transform.position + velocity);
                return info;
            }
            if (isUp)
            {
                var rem2 = GetMoveVector(rem1, hit1.normal);
                Vector3 vel2;
                var mov1 = vel1 + rem2;
                mov1.y = Mathf.Min(velocity.y, mov1.y);
                MoveTo(vel1 + GetMoveVector(rem1, hit1.normal), ignore, out vel2);
                body.MovePosition(transform.position + vel2);
                return info;
            }
            if (isDown)
            {
                info.Below = true;
                if (straightDown && CanStand(hit1.normal))
                {
                    body.MovePosition(transform.position + vel1);
                    return info;
                }
                var rem2 = GetMoveVector(rem1, hit1.normal);
                Vector3 vel2;
                var mov1 = vel1 + rem2;
                if (!CanStand(hit1.normal))
                {
                    rem2 = Vector3.Dot(rem2, down) > 0 ? rem2 : -rem2;
                    info.Below = false;
                }
                else
                {
                    mov1 = mov1 * (velocity.x / mov1.x);
                }
                var hit2 = MoveTo(mov1, ignore, out vel2);
                if (hit2.collider != null && CanStand(hit2.normal))
                {
                    info.Below = true;
                }
                body.MovePosition(transform.position + vel2);
                return info;
            }
            body.MovePosition(transform.position + vel1);
            return info;
        }

        public bool OnJumpThrough(Vector3 down, out Collider collider)
        {
            RaycastHit hit;
            if (GetComponent<Rigidbody>().SweepTest(down, out hit, skinWidth * 2))
            {
                collider = hit.collider;
                return Utils.IsInLayerMask(collider.gameObject.layer, oneWayLayer);
            }
            collider = null;
            return false;
        }
    }
}
