using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerJump : ActorState<PlayerActor>
    {

        public bool wallGrab = false;
        public bool pressed = false;
        public bool held = false;
        public bool control = true;
        public int count = 0;

        public float maxHeight = 4f;
        public float minHeight = 1f;
        public float timeToApex = 0.4f;
        public float maxJumps = 1;

        public float minSpeed;
        public float maxSpeed;

        public override void SetActor(PlayerActor actor)
        {
            base.SetActor(actor);
            actor.gravity = new Vector3(0, -(2 * maxHeight) / Mathf.Pow(timeToApex, 2));
            maxSpeed = Mathf.Abs(actor.gravity.y) * timeToApex;
            minSpeed = Mathf.Sqrt(2 * Mathf.Abs(actor.gravity.y) * minHeight);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            actor.UpdateVelocity(actor.velocity.x, maxSpeed);
            count += 1;
            wallGrab = false;
        }

        public override void Update()
        {
            base.Update();

            if (control)
            {
                actor.InputX();
                actor.AccelerateX();
            }

            actor.AccelerateY();

            if (!held && control)
            {
                actor.UpdateVelocity(actor.velocity.x, Mathf.Min(actor.velocity.y, minSpeed));
            }

            info = actor.Move();

            if (info.Left || info.Right)
            {
                actor.velocity.x = 0;
            }

            if (info.Above)
            {
                actor.velocity.y = 0;
            }

            if (actor.velocity.y <= 0)
            {
                actor.ChangeState(actor.states.Fall);
                return;
            }

        }

    }

}