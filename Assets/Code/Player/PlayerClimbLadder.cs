using System;
using System.Linq;

using KinematicCharacterController;
using UnityEngine;
using Assets.Code.Interactive;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerClimbLadder : PlayerState
    {

        [SerializeField]
        private string triggerName = "ladder-climb";
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

        public override void UpdateVelocity(ref Vector3 velocity, float deltaTime, KinematicCharacterMotor motor)
        {
            base.UpdateVelocity(ref velocity, deltaTime, motor);
            velocity = Ladder.transform.up * ClimbSpeed * Actor.input.y;
            if (Actor.input.y > 0)
            {
                motor.ForceUnground();
            }
            var v = Ladder.transform.right * -10 * Vector3.Dot(Ladder.transform.right, Actor.transform.position - Ladder.transform.position);
            if (v.magnitude > 0.25f)
            {
                velocity += v;
            }
        }

        public override void UpdateRotation(ref Quaternion rotation, float deltaTime, KinematicCharacterMotor motor)
        {
            rotation = Quaternion.Slerp(rotation, Quaternion.FromToRotation(Vector3.up, Ladder.transform.up), Mathf.Clamp01(Age * 5f));
        }

        public override void AfterUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.AfterUpdate(deltaTime, motor);
            if (motor.GroundingStatus.IsStableOnGround && Actor.input.y < 0)
            {
                Actor.ChangeState<PlayerIdle>();
                return;
            }

            if (Actor.GetState<PlayerJump>().pressed)
            {
                if (Actor.input.y >= 0)
                {
                    if (Mathf.Abs(Actor.input.x) > Actor.duckJoystickThreshold && Vector3.Dot(Actor.input, Ladder.transform.right) > 0)
                    {
                        var run = Actor.GetState<PlayerRun>();
                        Actor.velocity.x = run.maxSpeed * Mathf.Sign(Actor.input.x);
                        Actor.GetState<PlayerJump>().control = false;
                    }
                    Actor.ChangeState<PlayerJump>();
                }
                else if (Actor.input.y < -Actor.duckJoystickThreshold)
                {
                    var l = Actor.GetState<PlayerLedgeHang>();
                    var colliders = Ladder.GetComponentsInChildren<Collider>();
                    l.ignoreLedges.AddRange(
                        colliders
                        .Where(c => Utils.IsInLayerMask(c.gameObject.layer, Actor.SolidLayer))
                        .Select(c => Tuple.Create(c, Time.time + 0.5f))
                    );
                    Actor.ChangeState<PlayerFall>();
                }
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Ladder = null;
        }

    }
}
