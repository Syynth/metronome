using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerDuck : ActorState<PlayerActor>
    {

        public bool pressed = false;

        public override void OnEnter()
        {
            base.OnEnter();
        }

    }

}