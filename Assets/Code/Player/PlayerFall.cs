using UnityEngine;
using System;
using System.Linq;

using KinematicCharacterController;
using Assets.Code.Interactive;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerFall : PlayerState
    {

        [SerializeField]
        private string fallTriggerName = "fall";
        [SerializeField]
        private string descendTriggerName = "descend";

        public bool descend = true;
        public override string TriggerName => descend ? descendTriggerName : fallTriggerName;

        public float maxSpeed = 16f;
        public bool skipLedge = true;
        public bool touchingLedge = false;

        public GameObject LedgeDetect;

        public override void OnStart()
        {
            LedgeDetect = Actor.transform.Find("LedgeDetect").gameObject;
            var pos = LedgeDetect.transform.localPosition;
            pos.y = 4.5f;
            LedgeDetect.transform.localPosition = pos;
        }

        public override void BeforeUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.BeforeUpdate(deltaTime, motor);

            if (Age > 4f / 30f && Age < 1f / 3f)
            {
                skipLedge = false;
            }

            Actor.InputX();
            Actor.AccelerateX();
            Actor.AccelerateY();
        }

        public override void AfterUpdate(float deltaTime, KinematicCharacterMotor motor)
        {

            if (touchingLedge && Actor.input.y > 0)
            {
                Actor.ChangeState<PlayerLedgeHang>();
                return;
            }

            if (Actor.velocity.y > maxSpeed)
            {
                // TODO: Removed for Fans, not sure if needed?
                // Actor.UpdateVelocity(Actor.velocity.x, maxSpeed);
            }

            //if (info.Side)
            //{
            //    Debug.Log("info.Side == true");
            //    Actor.velocity.x = 0;
            //}

            if (Actor.GetState<PlayerLedgeHang>().DetectLedges(motor))
            {
                return;
            }

            var l = Actor.TouchingColliders.FirstOrDefault(c => c.GetComponent<Ladder>() != null);
            if (l != null)
            {
                Actor.GetState<PlayerClimbLadder>().Ladder = l.GetComponent<Ladder>();
                Actor.ChangeState<PlayerClimbLadder>();
                return;
            }

            if (motor.GroundingStatus.IsStableOnGround)
            {
                Actor.GetState<PlayerJump>().count = 0;
                if (Actor.input.x != 0)
                {
                    Actor.ChangeState<PlayerRun>();
                }
                else
                {
                    Actor.ChangeState<PlayerIdle>();
                }
                return;
            }

            var jump = Actor.GetState<PlayerJump>();
            if (jump.pressed)
            {
                if (jump.count < jump.maxJumps && jump.maxJumps > 1)
                {
                    Actor.ChangeState<PlayerJump>();
                    return;
                }
            }
        }

    }

}