using UnityEngine;
using System;
using Spine.Unity;
using KinematicCharacterController;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerRun : PlayerState
    {

        [SerializeField]
        private string triggerName = "run";
        public override string TriggerName => triggerName;

        public float aimWalkThreshold = 0.3f;
        public float walkThreshold = 0.65f;

        public float acceleration = 120f;
        public float friction = 80f;
        public float maxSpeed = 15f;
        public float accelerationTime = 0.2f;

        public float vMax = 32f;
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
                if (Actor.CurrentState == this)
                {
                    _down = value;
                    lastDown = Time.time;
                }
            }
        }
        public bool onGroundLastFrame = true;

        public float GetRunSpeed()
        {
            if (Actor.aiming)
            {
                return maxSpeed * aimWalkThreshold;
            }
            return !down ? maxSpeed * walkThreshold : maxSpeed;
        }

        bool CheckIdle()
        {
            if (Actor.input.x == 0 && Actor.velocity.x == 0)
            {
                Debug.Log("Check Idle passed");
                Actor.ChangeState<PlayerIdle>();
                return false;
            }
            return true;
        }

        public override void OnEnter(KinematicCharacterMotor motor)
        {
            base.OnEnter(motor);
            onGroundLastFrame = true;
            if (CheckIdle())
            {
                Actor.velocity.y = Actor.gravity.y * Time.deltaTime;
            }
        }

        public override void BeforeUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.BeforeUpdate(deltaTime, motor);
            Age += Time.deltaTime * (0.015f + Mathf.Abs(Actor.velocity.x / maxSpeed) * 0.985f);
            Actor.animator.SetFloat("run-speed", Mathf.Abs(Actor.velocity.x / maxSpeed));

            Actor.InputX();
            Actor.AccelerateX();
        }

        public override void UpdateVelocity(ref Vector3 velocity, KinematicCharacterMotor motor)
        {
            velocity = Actor.velocity;
        }

        public override void AfterUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.AfterUpdate(deltaTime, motor);
            //info = Actor.Move(onGroundLastFrame);

            if (Actor.GetState<PlayerJump>().pressed)
            {
                Actor.ChangeState<PlayerJump>();
                return;
            }

            //Collider collider;
            //if (Actor.states.Duck.held && Actor.motionController.OnJumpThrough(Actor.gravity, out collider))
            //{
            //    Actor.ignoreColliders.Add(Tuple.Create(collider, Time.time + 0.2f));
            //    Actor.states.Fall.descend = false;
            //    Actor.ChangeState(Actor.states.Fall);
            //    return;
            //}

            if (!CheckIdle()) return;

            if (!motor.GroundingStatus.IsStableOnGround)
            {
                if (onGroundLastFrame)
                {
                    onGroundLastFrame = false;
                }
                else
                {
                    Actor.GetState<PlayerFall>().descend = false;
                    Actor.ChangeState<PlayerFall>();
                    return;
                }
            }
            //Actor.rootBone.up = Vector3.Slerp(Actor.rootBone.up, info.GroundNormal, 0.2f);

        }

        public override void Render()
        {
            base.Render();
            var skeleton = Actor.GetComponentInChildren<SkeletonAnimator>().skeleton;
            if (Actor.aiming)
            {
                if (Actor.aimInput.x < 0)
                {
                    skeleton.flipX = true;
                }
                if (Actor.aimInput.x > 0)
                {
                    skeleton.flipX = false;
                }
                Actor.animator.SetFloat("walk-backwards", Mathf.Sign(Actor.aimInput.x) == Mathf.Sign(Actor.velocity.x) ? 0 : 1);
            }
            else
            {
                Actor.animator.SetFloat("walk-backwards", 0);
                if (Actor.velocity.x < 0)
                {
                    skeleton.flipX = true;
                }
                if (Actor.velocity.x > 0)
                {
                    skeleton.flipX = false;
                }
            }
        }

    }

}