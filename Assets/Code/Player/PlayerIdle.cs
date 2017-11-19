using Spine;
using Spine.Unity.Modules;
using Spine.Unity;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerIdle : PlayerState
    {
        [SerializeField]
        private string triggerName = "idle";
        public override string TriggerName => triggerName;

        public override void OnEnter()
        {
            base.OnEnter();
            actor.velocity.y = 0;
            actor.states.Jump.wallGrab = false;
            actor.GetComponentsInChildren<SkeletonUtilityGroundConstraint>().Select(c => c.enabled = true).ToArray();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            actor.GetComponentsInChildren<SkeletonUtilityGroundConstraint>().Select(c => c.enabled = false).ToArray();
        }

        bool CheckRun(IMotionController controller)
        {
            if (actor.input.x != 0)
            {
                var dx = Mathf.Sign(actor.input.x);
                var rightInfo = controller.CheckMove(Utils.Clockwise(actor.gravity.normalized), actor.gravity, actor.transform.position);
                if (dx > 0 && !rightInfo.Right) return true;
                var leftInfo = controller.CheckMove(Utils.CounterClockwise(actor.gravity.normalized), actor.gravity, actor.transform.position);
                if (dx < 0 && !leftInfo.Left) return true;
            }
            return false;
        }

        public override void Update()
        {
            base.Update();
            var controller = actor.GetComponent<IMotionController>();
            actor.velocity = actor.gravity * Time.deltaTime;
            var info = actor.Move();
            actor.InputX();

            if (actor.states.Jump.pressed)
            {
                actor.ChangeState(actor.states.Jump);
                return;
            }
            if (CheckRun(controller))
            {
                actor.ChangeState(actor.states.Run);
                return;
            }
            Collider collider;
            if (actor.states.Duck.held && controller.OnJumpThrough(actor.gravity, out collider))
            {
                actor.ignoreColliders.Add(Tuple.Create(collider, Time.time + 0.2f));
                actor.states.Fall.descend = false;
                actor.ChangeState(actor.states.Fall);
                return;
            }
            if (!info.Below)
            {
                actor.states.Fall.descend = false;
                actor.ChangeState(actor.states.Fall);
                return;
            }
            if (actor.input.y < 0 && info.Below)
            {
                actor.ChangeState(actor.states.Duck);
                return;
            }
            //actor.rootBone.up = Vector3.Slerp(actor.rootBone.up, Vector3.up, 0.3f);
        }

    }

}