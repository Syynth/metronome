using UnityEngine;

namespace Assets.Code.Player
{
    public class PlayerState : ActorState<PlayerActor>
    {

        public virtual string TriggerName => null;

        public override void OnEnter()
        {
            base.OnEnter();
            if (TriggerName != null)
            {
                // Debug.Log(string.Format("setting animation to {0} on frame {1}", TriggerName, actor.frame));
                actor.animator.SetTrigger(TriggerName);
            }
        }

    }
}
