using Spine;
using Spine.Unity.Modules;
using Spine.Unity;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using KinematicCharacterController;
using Assets.Code.Interactive;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerIdle : PlayerState
    {
        [SerializeField]
        private string triggerName = "Idle";
        public override string TriggerName => triggerName;

        public override void OnEnter(KinematicCharacterMotor motor)
        {
            base.OnEnter(motor);
            Actor.velocity.y = 0;
            Actor.GetState<PlayerJump>().wallGrab = false;
            Actor.GetComponentsInChildren<SkeletonUtilityGroundConstraint>().Select(c => c.enabled = true).ToArray();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Actor.GetComponentsInChildren<SkeletonUtilityGroundConstraint>().Select(c => c.enabled = false).ToArray();
        }

        bool CheckRun()
        {
            if (Actor.input.x != 0)
            {
                return true;
                //var dx = Mathf.Sign(Actor.input.x);
                //var rightInfo = controller.CheckMove(Utils.Clockwise(Actor.gravity.normalized), Actor.gravity, Actor.transform.position);
                //if (dx > 0 && !rightInfo.Right) return true;
                //var leftInfo = controller.CheckMove(Utils.CounterClockwise(Actor.gravity.normalized), Actor.gravity, Actor.transform.position);
                //if (dx < 0 && !leftInfo.Left) return true;
            }
            return false;
        }

        public override void BeforeUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.BeforeUpdate(deltaTime, motor);
            Actor.InputX();
        }

        public override void UpdateVelocity(ref Vector3 velocity, KinematicCharacterMotor motor)
        {
            base.UpdateVelocity(ref velocity, motor);
            velocity = Vector3.zero;
        }

        public override void AfterUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            if (Actor.input.y > Actor.duckJoystickThreshold)
            {
                var ladder = Actor.TouchingColliders.FirstOrDefault(c => c.gameObject.GetComponent<Ladder>());
                if (ladder != null)
                {
                    Actor.GetState<PlayerClimbLadder>().Ladder = ladder.GetComponent<Ladder>();
                    Actor.ChangeState<PlayerClimbLadder>();
                    return;
                }
            }

            if (Actor.GetState<PlayerJump>().pressed)
            {
                Actor.ChangeState<PlayerJump>();
                return;
            }

            if (CheckRun())
            {
                Actor.ChangeState<PlayerRun>();
                return;
            }
            Collider collider = motor.GroundingStatus.GroundCollider;
            if (collider != null && Actor.GetState<PlayerDuck>().held && Utils.IsInLayerMask(collider.gameObject.layer, Actor.OneWayLayer))
            {
                Actor.ignoreColliders.Add(Tuple.Create(collider, Time.time + 0.2f));
                Actor.GetState<PlayerFall>().descend = false;
                Actor.ChangeState<PlayerFall>();
                return;
            }
            if (!motor.GroundingStatus.IsStableOnGround)
            {
                Actor.GetState<PlayerFall>().descend = false;
                Actor.ChangeState<PlayerFall>();
                return;
            }
            //if (Actor.input.y < 0 && motor.GroundingStatus.IsStableOnGround)
            //{
            //    Actor.ChangeState<PlayerDuck>();
            //    return;
            //}
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
            }
            else
            {
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