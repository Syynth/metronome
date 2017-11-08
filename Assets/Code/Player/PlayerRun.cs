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
        public bool onGroundLastFrame = true;

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
            onGroundLastFrame = true;
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

            info = actor.Move(onGroundLastFrame);

            if (actor.states.Jump.pressed)
            {
                actor.ChangeState(actor.states.Jump);
                return;
            }
            Collider collider;
            if (actor.states.Duck.held && actor.motionController.OnJumpThrough(actor.gravity, out collider))
            {
                actor.ignoreColliders.Add(Tuple.Create(collider, Time.time + 1f));
                actor.ChangeState(actor.states.Fall);
                return;
            }

            if (!CheckIdle()) return;

            if (!info.Below)
            {
                if (onGroundLastFrame)
                {
                    onGroundLastFrame = false;
                }
                else
                {
                    actor.ChangeState(actor.states.Fall);
                    return;
                }
            }
            //actor.rootBone.up = Vector3.Slerp(actor.rootBone.up, info.GroundNormal, 0.2f);

        }

    }

}