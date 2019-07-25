using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using KinematicCharacterController;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerJump : PlayerState
    {

        [SerializeField]
        private string triggerName = "jump";
        public override string TriggerName => triggerName;

        public bool wallGrab = false;
        public bool pressed = false;
        public bool held = false;
        public bool control = true;
        public int count = 0;

        public float maxHeight = 5.75f;
        public float minHeight = 0.5f;
        public float timeToApex = 0.375f;
        public float maxJumps = 1;

        public float minSpeed;
        public float maxSpeed;

        public override void SetActor(PlayerActor Actor)
        {
            base.SetActor(Actor);
            Actor.gravity = new Vector3(0, -(2 * maxHeight) / Mathf.Pow(timeToApex, 2));
            maxSpeed = Mathf.Abs(Actor.gravity.y) * timeToApex;
            minSpeed = Mathf.Sqrt(2 * Mathf.Abs(Actor.gravity.y) * minHeight);
        }

        public override void OnEnter(KinematicCharacterMotor motor)
        {
            base.OnEnter(motor);
            Actor.velocity.y = maxSpeed;
            // Actor.UpdateVelocity(Actor.velocity.x, maxSpeed);
            count += 1;
            wallGrab = false;
            motor.ForceUnground();
        }

        public override void BeforeUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.BeforeUpdate(deltaTime, motor);
            base.AfterUpdate(deltaTime, motor);
            if (control)
            {
                Actor.InputX();
                Actor.AccelerateX();
            }

            Actor.AccelerateY();

            if (!held && control)
            {
                Actor.UpdateVelocity(Actor.velocity.x, Mathf.Min(Actor.velocity.y, minSpeed));
            }
        }

        public override void AfterUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            if (Actor.velocity.y <= 0)
            {
                Actor.GetState<PlayerFall>().descend = true;
                Actor.ChangeState<PlayerFall>();
                return;
            }

            if (Age < 0.5f)
            {
                return; // Skip ledge detect checks for the same ledge you're already on
            }

            if (Actor.GetState<PlayerLedgeHang>().DetectLedges(motor))
            {
                return;
            }
            //Actor.rootBone.up = Vector3.Slerp(Actor.rootBone.up, Vector3.up, 0.2f);
        }

    }

}