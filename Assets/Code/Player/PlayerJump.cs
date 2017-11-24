using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerJump : PlayerState
    {

        [SerializeField]
        private string triggerName = "jump";
        public override string TriggerName => triggerName;

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
            actor.velocity.y = maxSpeed;
            // actor.UpdateVelocity(actor.velocity.x, maxSpeed);
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

            if (info.Side)
            {
                Debug.Log("Setting velocity.x to 0");
                actor.velocity.x = 0;
            }

            if (info.Above)
            {
                actor.velocity.y = 0;
            }

            if (actor.velocity.y <= 0)
            {
                actor.states.Fall.descend = true;
                actor.ChangeState(actor.states.Fall);
                return;
            }

            var hits = actor.states.Fall.LedgeDetect.GetComponent<Rigidbody>()
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
            //actor.rootBone.up = Vector3.Slerp(actor.rootBone.up, Vector3.up, 0.2f);
        }

    }

}