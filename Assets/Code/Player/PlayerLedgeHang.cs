using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerLedgeHang : ActorState<PlayerActor>
    {

        public bool pressed = false;
        public bool held = false;

        public override void OnEnter()
        {
            base.OnEnter();
            actor.states.Jump.count = 0;
        }

        public override void Update()
        {
            actor.InputX();
            if (actor.states.Jump.pressed)
            {
                actor.ChangeState(actor.states.Jump);
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            actor.velocity.x = 0;
        }

    }

}