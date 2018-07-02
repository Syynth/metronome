using UnityEngine;
using System.Collections;
using System;
using KinematicCharacterController;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerDuck : PlayerState
    {
        [SerializeField]
        private string triggerName = "Crouch";
        public override string TriggerName => triggerName;

        public bool pressed = false;
        public bool held = false;

        public float duckHeight = 3;
        
        private Vector2 startSize;
        private Vector2 startCenter;

        public override void OnStart()
        {
            base.OnStart();
            startSize = Vector2.zero;
            startCenter = Vector2.zero;
        }

        public override void BeforeUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.BeforeUpdate(deltaTime, motor);
        }

        public override void UpdateVelocity(ref Vector3 velocity, KinematicCharacterMotor motor)
        {
            Actor.InputX();
            Actor.velocity.x = 0;
            //collider.size = new Vector2(startSize.x, Mathf.Lerp(startSize.y, duckHeight, Age * 10f));
            //collider.center = new Vector2(startCenter.x, Mathf.Lerp(startCenter.y, startCenter.y - (startSize.y / 2 - duckHeight / 2), Age * 10f));
            if (Actor.input.x != 0)
            {
                Actor.velocity.x = Mathf.Sign(Actor.input.x);
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
                Actor.ChangeState<PlayerIdle>();
                return;
            }
            if (Actor.GetState<PlayerJump>().pressed)
            {
                Actor.ChangeState<PlayerJump>();
                return;
            }
            //Actor.rootBone.up = Vector3.Slerp(Actor.rootBone.up, Vector3.up, 0.2f);
        }

        public override void OnExit()
        {
            base.OnExit();
            Actor.velocity.x = 0;
            //collider.size = startSize;
            //collider.center = startCenter;
        }

    }

}