using UnityEngine;
using System;
using Spine.Unity;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerRun : PlayerState
    {

        [SerializeField]
        private string triggerName = "run";
        public override string TriggerName => triggerName;

        public float aimWalkThreshold = 0.3f;
        public float walkThreshold = 0.45f;

        public float acceleration = 40f;
        public float friction = 34f;
        public float maxSpeed = 10f;
        public float accelerationTime = 0.2f;

        public float vMax = 16f;
        public bool xPressed = false;

        [SerializeField]
        private bool _down = false;
        private float lastDown = 0;
        public bool down
        {
            get
            {
                return _down;
            }
            set
            {
                if (actor.CurrentState == this)
                {
                    _down = value;
                    lastDown = Time.time;
                }
            }
        }
        public bool onGroundLastFrame = true;

        public float GetRunSpeed()
        {
            if (actor.aiming)
            {
                return maxSpeed * aimWalkThreshold;
            }
            return !down ? maxSpeed * walkThreshold : maxSpeed;
        }

        bool CheckIdle()
        {
            if (actor.input.x == 0 && actor.velocity.x == 0)
            {
                Debug.Log("Check Idle passed");
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
            actor.animator.SetFloat("run-speed", Mathf.Abs(actor.velocity.x / maxSpeed));

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
                actor.ignoreColliders.Add(Tuple.Create(collider, Time.time + 0.2f));
                actor.states.Fall.descend = false;
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
                    actor.states.Fall.descend = false;
                    actor.ChangeState(actor.states.Fall);
                    return;
                }
            }
            //actor.rootBone.up = Vector3.Slerp(actor.rootBone.up, info.GroundNormal, 0.2f);

        }

        public override void Render()
        {
            base.Render();
            var skeleton = actor.GetComponentInChildren<SkeletonAnimator>().skeleton;
            if (actor.aiming)
            {
                if (actor.aimInput.x < 0)
                {
                    skeleton.flipX = true;
                }
                if (actor.aimInput.x > 0)
                {
                    skeleton.flipX = false;
                }
                actor.animator.SetFloat("walk-backwards", Mathf.Sign(actor.aimInput.x) == Mathf.Sign(actor.velocity.x) ? 0 : 1);
            }
            else
            {
                if (actor.velocity.x < 0)
                {
                    skeleton.flipX = true;
                }
                if (actor.velocity.x > 0)
                {
                    skeleton.flipX = false;
                }
            }
        }

    }

}