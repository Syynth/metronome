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

        public override void OnEnter()
        {
            base.OnEnter();
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