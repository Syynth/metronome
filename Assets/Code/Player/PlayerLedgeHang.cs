﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Spine.Unity;

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

        public override void OnEnter()
        {
            base.OnEnter();
            actor.states.Jump.count = 0;
            actor.velocity.y = 0;
            //actor.states.Jump.count = 0;
            //actor.GetComponentsInChildren<SkeletonUtilityBone>()
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

        public override void Update()
        {
            actor.InputX();
            if (actor.states.Jump.pressed)
            {
                ignoreLedges.Add(Tuple.Create(Ledge, Time.time + 0.08f));
                actor.ChangeState(actor.states.Jump);
                return;
            }
            if (actor.states.Duck.pressed)
            {
                ignoreLedges.Add(Tuple.Create(Ledge, Time.time + 0.08f));
                actor.ChangeState(actor.states.Fall);
                return;
            }
            //actor.rootBone.up = Vector3.Slerp(actor.rootBone.up, Vector3.up, 0.2f);
        }

        public override void OnExit()
        {
            base.OnExit();
            actor.velocity.x = 0;
            actor.GetComponentsInChildren<SkeletonUtilityBone>().Where(c => c.mode == SkeletonUtilityBone.Mode.Override && c.gameObject.name.Contains("Arm")).Select(c => c.enabled = false).ToArray();
        }

    }

}