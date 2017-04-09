using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerRun : ActorState<PlayerActor>
    {

        public SpriteAnimation walkAnimation;
        public SpriteAnimation jogAnimation;
        public SpriteAnimation startUp;

        public float acceleration = 1f;
        public float friction = 0.5f;
        public float maxSpeed = 10f;

        public float vMax = 10f;
        public bool pressed = false;


        public override void OnEnter()
        {
            base.OnEnter();
        }

    }

}