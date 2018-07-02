using System;

using KinematicCharacterController;
using UnityEngine;
using Assets.Code.Interactive;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerClimbLadder : PlayerState
    {

        [SerializeField]
        private string triggerName = "Ladder";
        public override string TriggerName => triggerName;

        public Ladder Ladder;
        public float ClimbSpeed = 3f;

        public override void OnEnter(KinematicCharacterMotor motor)
        {
            base.OnEnter(motor);
        }

        public override void BeforeUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.BeforeUpdate(deltaTime, motor);
            Actor.InputX();
        }

        public override void UpdateVelocity(ref Vector3 velocity, KinematicCharacterMotor motor)
        {
            base.UpdateVelocity(ref velocity, motor);
            velocity = Ladder.transform.up * ClimbSpeed * Actor.input.y;
            if (Actor.input.y > 0)
            {
                motor.ForceUnground();
            }
        }

        public override void AfterUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.AfterUpdate(deltaTime, motor);
            if (motor.GroundingStatus.IsStableOnGround)
            {
                Actor.ChangeState<PlayerIdle>();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Ladder = null;
        }

    }
}
