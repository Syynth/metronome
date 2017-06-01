using UnityEngine;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerRun : ActorState<PlayerActor>
    {

        public float walkThreshold = 0.3f;
        public float jogThreshold = 0.8f;

        public float acceleration = 40f;
        public float friction = 34f;
        public float maxSpeed = 10f;

        public float vMax = 16f;
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
            Age += Time.deltaTime * (0.015f + Mathf.Abs(actor.velocity.x / maxSpeed) * 0.985f);

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

        //public override void Render()
        //{
        //    return;
        //    SpriteAnimation anim = animation;
        //    if (Mathf.Abs(actor.velocity.x / maxSpeed) < jogThreshold)
        //    {
        //        anim = jogAnimation;
        //    }
        //    if (Mathf.Abs(actor.velocity.x / maxSpeed) < walkThreshold)
        //    {
        //        anim = walkAnimation;
        //    }
        //    actor.GetComponent<SpriteRenderer>().sprite = anim.getFrame(Age);
        //}

    }

}