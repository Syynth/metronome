using UnityEngine;
using System.Collections;
using System;

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

        private void TouchLedge(Collision2D collision)
        {
            if (Utils.IsInLayerMask(collision.gameObject.layer, actor.motionController.SolidLayer))
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