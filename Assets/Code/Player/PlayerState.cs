using Assets.Code.States;
using KinematicCharacterController;
using UnityEngine;

namespace Assets.Code.Player
{
    public abstract class PlayerState : ActorState<PlayerActor>
    {

        public virtual string TriggerName => null;

        public override void OnEnter(KinematicCharacterMotor motor)
        {
            base.OnEnter(motor);
            if (TriggerName != null)
            {
                //Debug.Log(string.Format("setting animation to {0} on frame {1}", TriggerName, actor.frame));
                Actor.animator.SetTrigger(TriggerName);
            }
        }

        public override void UpdateVelocity(ref Vector3 velocity, float deltaTime, KinematicCharacterMotor motor)
        {
            velocity = Actor.velocity;
        }

        public override void UpdateRotation(ref Quaternion rotation, float deltaTime, KinematicCharacterMotor motor)
        {
            rotation = Quaternion.Slerp(rotation, Quaternion.FromToRotation(Vector3.zero, Vector3.up), Mathf.Clamp01(Age * 10f));
        }

    }
}
