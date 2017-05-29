using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code.Player
{

    [Serializable]
    public class PlayerIdle : ActorState<PlayerActor>
    {

        CollisionInfo info;

        public override void OnEnter()
        {
            base.OnEnter();
            actor.velocity.y = 0;
            actor.states.Jump.wallGrab = false;
        }

        bool CheckRun(BoxcastMotionController controller)
        {
            if (actor.input.x != 0)
            {
                var dx = Mathf.Sign(actor.input.x);
                var rightInfo = controller.CheckMove(controller.Clockwise(actor.gravity.normalized), actor.gravity, actor.transform.position);
                if (dx > 0 && !rightInfo.Right) return true;
                var leftInfo = controller.CheckMove(controller.CounterClockwise(actor.gravity.normalized), actor.gravity, actor.transform.position);
                if (dx < 0 && !leftInfo.Left) return true;
            }
            return false;
        }

        public override void Update()
        {
            base.Update();
            var controller = actor.GetComponent<BoxcastMotionController>();
            actor.velocity = actor.gravity * Time.deltaTime;
            info = actor.Move();
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
            if (actor.states.Duck.pressed && controller.OnJumpThrough(actor.gravity))
            {
                actor.transform.position += actor.gravity.normalized * RaycastController.skinWidth * 2;
                actor.ChangeState(actor.states.Fall);
                return;
            }
            if (!info.Below)
            {
                actor.ChangeState(actor.states.Fall);
                return;
            }
            if (actor.input.y < 0 && info.Below)
            {
                actor.ChangeState(actor.states.Duck);
                return;
            }

        }

    }

}