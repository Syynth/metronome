using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerRun : ActorState<PlayerActor>
    {

        public SpriteAnimation walkAnimation;
        public SpriteAnimation jogAnimation;
        public SpriteAnimation startUp;

        public float acceleration = 10f;
        public float friction = 0.5f;
        public float maxSpeed = 10f;

        public float vMax = 10f;
        public bool pressed = false;

        bool CheckIdle()
        {
            if (actor.input.x == 0 && actor.velocity.x == 0)
            {
                actor.ChangeState(actor.states.Idle);
                return false;
            }
            return true;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (CheckIdle())
            {
                actor.velocity.y = actor.gravity.y * Time.deltaTime;
            }
        }

        public override void Update()
        {
            base.Update();

            actor.InputX();
            actor.AccelerateX();

            info = actor.Move();

            if (actor.states.Jump.pressed)
            {
                actor.ChangeState(actor.states.Jump);
                return;
            }

            if (!CheckIdle()) return;

            if (!info.Below)
            {
                actor.ChangeState(actor.states.Fall);
                return;
            }

        }

    }

}