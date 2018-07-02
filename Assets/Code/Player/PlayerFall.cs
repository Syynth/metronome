using UnityEngine;
using System;
using System.Linq;

using KinematicCharacterController;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerFall : PlayerState
    {

        [SerializeField]
        private string fallTriggerName = "Fall";
        [SerializeField]
        private string descendTriggerName = "Fall";

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

        public override void UpdateVelocity(ref Vector3 velocity, KinematicCharacterMotor motor)
        {
            base.UpdateVelocity(ref velocity, motor);

            if (Age > 4f / 30f && Age < 1f / 3f)
            {
                skipLedge = false;
            }

            Actor.InputX();
            Actor.AccelerateX();
            Actor.AccelerateY();

            velocity = Actor.velocity;

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

            var hits = LedgeDetect.GetComponent<Rigidbody>()
                .SweepTestAll(Vector3.down, 0.3f)
                .Where(
                    hit => !Actor.GetState<PlayerLedgeHang>().ignoreLedges
                    .Select(t => t.Item1)
                    .Contains(hit.collider)
                );
            if (hits.Count() > 0)
            {
                var hit = hits.First();
                if (motor.IsStableOnNormal(hit.normal) && Utils.IsInLayerMask(hit.collider.gameObject.layer, Actor.SolidLayer))
                {
                    Actor.GetState<PlayerLedgeHang>().Ledge = hit.collider;
                    Actor.ChangeState<PlayerLedgeHang>();
                    return;
                }
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