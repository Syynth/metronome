using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Spine.Unity;
using KinematicCharacterController;
using Assets.Code.Interactive;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerLedgeHang : PlayerState
    {

        [SerializeField]
        private string triggerName = "ledge-hang";
        public override string TriggerName => triggerName;
        public List<Tuple<Collider, float>> ignoreLedges = new List<Tuple<Collider, float>>();

        public Collider Ledge;

        public bool pressed = false;
        public bool held = false;

        public bool grabOneWayPlatforms = true;
        public Vector3 ledgeVertex;

        public bool GrabbingLadder;

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

        public override void BeforeUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.BeforeUpdate(deltaTime, motor);
            Actor.InputX();
        }

        public override void UpdateVelocity(ref Vector3 velocity, float deltaTime, KinematicCharacterMotor motor)
        {
            velocity = Vector3.zero;
        }

        public override void AfterUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            base.AfterUpdate(deltaTime, motor);

            if (Mathf.Abs(Actor.input.y) > Actor.duckJoystickThreshold)
            {
                var ladder = Actor.GetState<PlayerClimbLadder>().Ladder; ; 
                if (ladder != null && GrabbingLadder)
                {
                    Actor.ChangeState<PlayerClimbLadder>();
                    return;
                }
            }            
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
        }

        public override void OnExit()
        {
            base.OnExit();
            GrabbingLadder = false;
            Actor.velocity.x = 0;
            Actor.GetComponentsInChildren<SkeletonUtilityBone>()
                .Where(c =>
                    c.mode == SkeletonUtilityBone.Mode.Override &&
                    c.gameObject.name.Contains("Arm"))
                .Select(c => c.enabled = false)
                .ToArray();
        }

        public bool DetectLedges(KinematicCharacterMotor motor)
        {
            var ld = Actor.GetState<PlayerFall>().LedgeDetect;
            var r = ld.GetComponent<Rigidbody>();
            var hits = r
                .SweepTestAll(Vector3.down, 0.3f, QueryTriggerInteraction.Collide)
                .Where(
                    hit => !Actor.GetState<PlayerLedgeHang>().ignoreLedges
                    .Select(t => t.Item1)
                    .Contains(hit.collider)
                ).ToArray();
            if (hits.Count() > 0)
            {
                //Debug.Break();
                var hit = hits.FirstOrDefault(h => Utils.IsInLayerMask(h.collider.gameObject.layer, Actor.SolidLayer));
                if (motor.IsStableOnNormal(hit.normal) && hit.collider != null)
                {
                    var lh = Actor.GetState<PlayerLedgeHang>();
                    lh.Ledge = hit.collider;
                    var lc = ld.GetComponent<BoxCollider>();
                    var overlaps = Physics.OverlapBox(
                        r.transform.position,
                        lc.size / 2,
                        ld.transform.rotation,
                        1 << LayerMask.NameToLayer("Ladder"),
                        QueryTriggerInteraction.Collide
                    );
                    Debug.Log($"Found {overlaps.Length} ladder triggers, with size {lc.size / 2}");
                    var ladder = overlaps.Select(h => h?.gameObject.GetComponent<Ladder>()).FirstOrDefault(l => l != null);
                    if (ladder != null)
                    {
                        Debug.Log("Ladder layer collider had ladder component attached");
                        lh.GrabbingLadder = true;
                        Actor.GetState<PlayerClimbLadder>().Ladder = ladder;
                    }
                    Actor.ChangeState<PlayerLedgeHang>();
                    return true;
                }
            }
            return false;
        }

    }

}