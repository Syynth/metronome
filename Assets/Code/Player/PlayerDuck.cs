using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerDuck : PlayerState
    {
        [SerializeField]
        private string triggerName = "duck";
        public override string TriggerName => triggerName;

        public bool pressed = false;
        public bool held = false;

        public float duckHeight = 3;

        private BoxCollider collider;
        private Vector2 startSize;
        private Vector2 startCenter;

        public override void OnStart()
        {
            base.OnStart();
            collider = actor.GetComponent<BoxCollider>();
            startSize = collider.size;
            startCenter = collider.center;
        }

        public override void Update()
        {
            actor.InputX();
            actor.velocity.x = 0;
            collider.size = new Vector2(startSize.x, Mathf.Lerp(startSize.y, duckHeight, Age * 10f));
            collider.center = new Vector2(startCenter.x, Mathf.Lerp(startCenter.y, startCenter.y - (startSize.y / 2 - duckHeight / 2), Age * 10f));
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
            //actor.rootBone.up = Vector3.Slerp(actor.rootBone.up, Vector3.up, 0.2f);
        }

        public override void OnExit()
        {
            base.OnExit();
            actor.velocity.x = 0;
            collider.size = startSize;
            collider.center = startCenter;
        }

    }

}