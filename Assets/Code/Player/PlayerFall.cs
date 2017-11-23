using UnityEngine;
using System.Collections;
using System;
using System.Linq;

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
            var pos = LedgeDetect.transform.localPosition;
            pos.y = 4.5f;
            LedgeDetect.transform.localPosition = pos;
        }

        public override void Update()
        {
            base.Update();

            if (Age > 4f / 30f && Age < 1f / 3f)
            {
                skipLedge = false;
            }

            actor.InputX();
            actor.AccelerateX();
            actor.AccelerateY();

            if (touchingLedge && actor.input.y > 0)
            {
                actor.ChangeState(actor.states.LedgeHang);
                return;
            }

            if (actor.velocity.y > maxSpeed)
            {
                actor.UpdateVelocity(actor.velocity.x, maxSpeed);
            }

            info = actor.Move();

            if (info.Side)
            {
                Debug.Log("info.Side == true");
                actor.velocity.x = 0;
            }

            var hits = LedgeDetect.GetComponent<Rigidbody>()
                .SweepTestAll(Vector3.down, 0.3f)
                .Where(
                    hit => !actor.states.LedgeHang.ignoreLedges
                    .Select(t => t.Item1)
                    .Contains(hit.collider)
                );
            if (hits.Count() > 0)
            {
                var hit = hits.First();
                if (actor.motionController.CanStand(hit.normal) && Utils.IsInLayerMask(hit.collider.gameObject.layer, actor.motionController.SolidLayer))
                {
                    actor.states.LedgeHang.Ledge = hit.collider;
                    actor.ChangeState(actor.states.LedgeHang);
                    return;
                }
            }

            if (info.Below)
            {
                actor.states.Jump.count = 0;
                if (actor.input.x != 0)
                {
                    actor.ChangeState(actor.states.Run);
                }
                else
                {
                    actor.ChangeState(actor.states.Idle);
                }
                return;
            }

            if (actor.states.Jump.pressed)
            {
                if (actor.states.Jump.count < actor.states.Jump.maxJumps && actor.states.Jump.maxJumps > 1)
                {
                    actor.ChangeState(actor.states.Jump);
                    return;
                }
            }
        }

    }

}