using Assets.Code.States;
using KinematicCharacterController;

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

    }
}
