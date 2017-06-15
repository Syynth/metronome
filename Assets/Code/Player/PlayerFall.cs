using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerFall : ActorState<PlayerActor>
    {

        public float maxSpeed = 16f;
        public bool skipLedge = true;
        public bool touchingLedge = false;

        public CollisionEvents LedgeDetect;

        public override void OnStart()
        {
            var pos = LedgeDetect.transform.localPosition;
            LedgeDetect.OnCollisionEnter += TouchLedge;
            LedgeDetect.OnCollisionExit += StopTouchLedge;
            pos.y = 5f;
            LedgeDetect.transform.localPosition = pos;
        }

        bool CloseEnough(Vector2 p)
        {
            return Vector2.Distance(LedgeDetect.transform.position, p) < LedgeDetect.GetComponent<CircleCollider2D>().radius;
        }

        Vector2 GetUpVector(Vector2 a, Vector2 b)
        {
            var dist = b - a;
            var normal = Utils.Clockwise(dist);
            return (Vector2.Dot(normal, Vector2.up) > 0) ? normal : -normal;
        }

        bool WithinAngle(int index, Vector2[] points)
        {
            var point = points[index];
            var left = points[Utils.Mod(index - 1, points.Length)];
            var right = points[Utils.Mod(index + 1, points.Length)];
            if (Vector2.Angle(left - point, right - point) < 120)
            {
                return actor.motionController.CanStand(GetUpVector(point, left)) || actor.motionController.CanStand(GetUpVector(point, right));
            }
            return false;
        }

        private bool ValidLedgeContact(Collider2D collider)
        {
            if (collider is EdgeCollider2D)
            {
                var effector = collider.gameObject.GetComponent<PlatformEffector2D>();
                if (effector != null && effector.useOneWay)
                {
                    return actor.states.LedgeHang.grabOneWayPlatforms;
                }
                var c = collider as EdgeCollider2D;
                
                return c.points
                    .Select(p => collider.transform.TransformPoint(p))
                    .Where((p, i) => CloseEnough(p) && WithinAngle(i, c.points)).Count() > 0;
            }
            if (collider is PolygonCollider2D)
            {
                var effector = collider.gameObject.GetComponent<PlatformEffector2D>();
                if (effector != null && effector.useOneWay)
                {
                    return actor.states.LedgeHang.grabOneWayPlatforms;
                }
                var c = collider as PolygonCollider2D;

                return c.points
                    .Select(p => collider.transform.TransformPoint(p))
                    .Where((p, i) => CloseEnough(p) && WithinAngle(i, c.points)).Count() > 0;
            }
            return false;
        }

        private void TouchLedge(Collision2D collision)
        {
            if (Utils.IsInLayerMask(collision.gameObject.layer, actor.motionController.SolidLayer) && ValidLedgeContact(collision.collider))
            {
                touchingLedge = true;
            }
        }

        private void StopTouchLedge(Collision2D collision)
        {
            if (Utils.IsInLayerMask(collision.gameObject.layer, actor.motionController.SolidLayer))
            {
                touchingLedge = false;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();

            if (Age > 4f / 30f && Age < 1f / 3f)
            {
                skipLedge = false;
            }

            actor.InputX();
            actor.AccelerateX();
            actor.AccelerateY();

            if (touchingLedge && actor.input.y > 0)
            {
                actor.ChangeState(actor.states.LedgeHang);
                return;
            }

            if (actor.velocity.y > maxSpeed)
            {
                actor.UpdateVelocity(actor.velocity.x, maxSpeed);
            }

            info = actor.Move();

            if (info.Left || info.Right)
            {
                actor.velocity.x = 0;
            }

            if (info.Below)
            {
                actor.states.Jump.count = 0;
                if (actor.input.x != 0)
                {
                    actor.ChangeState(actor.states.Run);
                }
                else
                {
                    actor.ChangeState(actor.states.Idle);
                }
                return;
            }

            if (actor.states.Jump.pressed)
            {
                if (actor.states.Jump.count < actor.states.Jump.maxJumps && actor.states.Jump.maxJumps > 1)
                {
                    actor.ChangeState(actor.states.Jump);
                    return;
                }
            }

        }

    }

}