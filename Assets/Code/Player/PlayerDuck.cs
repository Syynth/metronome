using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerDuck : ActorState<PlayerActor>
    {

        public bool pressed = false;
        public bool held = false;

        public override void Update()
        {
            actor.InputX();
            actor.velocity.x = 0;
            if (actor.input.x != 0)
            {

                actor.velocity.x = Mathf.Sign(actor.input.x);
            }
            if (held)
            {
                Age += Time.deltaTime;
                Age = Mathf.Min(Age, 0.1f);
            }
            else
            {
                Age -= Time.deltaTime;
                Age = Mathf.Max(Age, 0);
            }
            if (Age == 0)
            {
                actor.ChangeState(actor.states.Idle);
                return;
            }
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

        //public override void Render()
        //{
        //    Sprite sprite;
        //    if (Age == 0.1f)
        //    {
        //        sprite = animation.sprites[1];
        //    }
        //    else
        //    {
        //        sprite = animation.sprites[0];
        //    }
        //    actor.GetComponent<SpriteRenderer>().sprite = sprite;
        //}

    }

}