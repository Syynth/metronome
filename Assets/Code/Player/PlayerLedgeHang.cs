using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Spine.Unity;
using KinematicCharacterController;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerLedgeHang : PlayerState
    {

        [SerializeField]
        private string triggerName = "LedgeHang";
        public override string TriggerName => triggerName;
        public List<Tuple<Collider, float>> ignoreLedges = new List<Tuple<Collider, float>>();

        public Collider Ledge;

        public bool pressed = false;
        public bool held = false;

        public bool grabOneWayPlatforms = true;
        public Vector3 ledgeVertex;

        public override void OnEnter(KinematicCharacterMotor motor)
        {
            base.OnEnter(motor);
            Actor.GetState<PlayerJump>().count = 0;
            Actor.velocity.y = 0;
            //Actor.states.Jump.count = 0;
            //Actor.GetComponentsInChildren<SkeletonUtilityBone>()
            //    .Where(c => c.mode == SkeletonUtilityBone.Mode.Override && c.gameObject.name.Contains("Arm"))
            //    .Select(c =>
            //    {
            //        c.enabled = true;
            //        c.transform.position = ledgeVertex;
            //        return c;
            //    })
            //    .ToArray();
        }

        public override void Tick()
        {
            base.Tick();
            ignoreLedges = ignoreLedges.Where(tuple => Time.time < tuple.Item2).ToList();
        }

        public override void UpdateVelocity(ref Vector3 velocity, KinematicCharacterMotor motor)
        {
            base.UpdateVelocity(ref velocity, motor);
            velocity = Vector3.zero;
        }

        public override void AfterUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.AfterUpdate(deltaTime, motor);

            Actor.InputX();
            if (Actor.GetState<PlayerJump>().pressed)
            {
                ignoreLedges.Add(Tuple.Create(Ledge, Time.time + 0.08f));
                Actor.ChangeState<PlayerJump>();
                return;
            }
            if (Actor.GetState<PlayerDuck>().pressed)
            {
                ignoreLedges.Add(Tuple.Create(Ledge, Time.time + 0.08f));
                Actor.ChangeState<PlayerFall>();
                return;
            }
            //Actor.rootBone.up = Vector3.Slerp(Actor.rootBone.up, Vector3.up, 0.2f);
        }

        public override void OnExit()
        {
            base.OnExit();
            Actor.velocity.x = 0;
            Actor.GetComponentsInChildren<SkeletonUtilityBone>()
                .Where(c =>
                    c.mode == SkeletonUtilityBone.Mode.Override &&
                    c.gameObject.name.Contains("Arm"))
                .Select(c => c.enabled = false)
                .ToArray();
        }

    }

}