using Spine.Unity;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Code.Player
{

    [RequireComponent(typeof(IMotionController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerActor : MonoBehaviour, IStateMachine<PlayerActor>
    {

        public Vector3 velocity;
        public Vector3 gravity = Vector3.down * 10;
        public Vector2 input = Vector2.zero;

        public IMotionController motionController;
        //public Transform rootBone;

        public PlayerStates states;
        public List<Tuple<Collider, float>> ignoreColliders;
        public Animator animator;

        #region IStateMachine Implementation

        public ActorState<PlayerActor> CurrentState { get; set; }
        public ActorState<PlayerActor> PreviousState { get; set; }
        public ActorState<PlayerActor> NextState { get; set; }

        public void ChangeState(ActorState<PlayerActor> nextState)
        {
            if (nextState == CurrentState) return;
            NextState = nextState;
            CurrentState.OnExit();
            PreviousState = CurrentState;
            CurrentState = nextState;
            NextState = null;
            CurrentState.OnEnter();
        }

        #endregion

        void Start()
        {
            ignoreColliders = new List<Tuple<Collider, float>>();
            motionController = GetComponents<IMotionController>()
                .Select(mc => mc as MonoBehaviour)
                .Where(c => c.enabled)
                .Select(c => c as IMotionController).First();
            GetComponent<Rigidbody>().isKinematic = true;
            animator = GetComponentInChildren<Animator>();

            states.Duck.SetActor(this);
            states.Fall.SetActor(this);
            states.Idle.SetActor(this);
            states.Jump.SetActor(this);
            states.Run.SetActor(this);
            states.LedgeHang.SetActor(this);

            states.Duck.OnStart();
            states.Fall.OnStart();
            states.Idle.OnStart();
            states.Jump.OnStart();
            states.Run.OnStart();
            states.LedgeHang.OnStart();

            CurrentState = states.Idle;
            PreviousState = states.Idle;
            CurrentState.OnEnter();
        }

        void FixedUpdate()
        {
            CurrentState.Update();
            CurrentState.Render();
            var skeleton = GetComponentInChildren<SkeletonAnimator>().skeleton;
            //var skeleton = GetComponent<SkeletonAnimation>().skeleton;
            ignoreColliders = ignoreColliders.Where(pair => pair.Item2 > Time.time).ToList();
            var pos = states.Fall.LedgeDetect.transform.localPosition;
            if (velocity.x < 0)
            {
                skeleton.flipX = true;
                pos.x = -1.7f / 2;
            }
            if (velocity.x > 0)
            {
                skeleton.flipX = false;
                pos.x = 1.7f / 2;
            }
            states.Fall.LedgeDetect.transform.localPosition = pos;
        }

        public void InputX()
        {
            bool downReleased = input.y >= 0;
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            states.Run.pressed = input.x != 0;
            var held = states.Jump.held;
            states.Jump.held = Input.GetButton("Jump");
            states.Jump.pressed = states.Jump.held && !held;
            states.Duck.pressed = input.y < 0 && downReleased;
            states.Duck.held = input.y < 0;
            var sign = Math.Sign(input.x);
            if (sign == Direction.Right)
            {
                states.Run.vMax = states.Run.maxSpeed;
            }
            else if (sign == Direction.Left)
            {
                states.Run.vMax = -states.Run.maxSpeed;
            }
        }

        public void AccelerateX()
        {
            var dx = states.Run.acceleration * Mathf.Sign(states.Run.vMax) * Time.deltaTime;
            if (states.Run.pressed)
            {
                // Accumulate speed when traveling
                velocity.x += dx;

                // Bonus speed if you're going slow
                if (Mathf.Abs(velocity.x) < states.Run.maxSpeed * 0.5f)
                {
                    velocity.x += dx;
                }

                if (Mathf.Abs(velocity.x) > states.Run.maxSpeed)
                {
                    velocity.x = states.Run.vMax;
                }
            }
            if (Mathf.Abs(velocity.x) < states.Run.friction * Time.deltaTime)
            {
                velocity.x = 0;
            }
            else
            {
                velocity.x -= states.Run.friction * Mathf.Sign(velocity.x) * Time.deltaTime;
            }
        }

        public void AccelerateY()
        {
            velocity += gravity * Time.deltaTime;
        }

        public void UpdateVelocity(float x, float y)
        {
            velocity.x = x;
            velocity.y = y;
        }

        public CollisionInfo Move()
        {
            return Move(velocity * Time.deltaTime);
        }

        public CollisionInfo Move(bool findGround)
        {
            return motionController.Move(velocity * Time.deltaTime, gravity, ignoreColliders.Select(p => p.Item1).ToList(), findGround);
        }

        public CollisionInfo Move(Vector3 velocity)
        {
            return motionController.Move(velocity, gravity, ignoreColliders.Select(p => p.Item1).ToList());
        }

    }

    [Serializable]
    public struct PlayerStates
    {
        public PlayerIdle Idle;
        public PlayerJump Jump;
        public PlayerFall Fall;
        public PlayerDuck Duck;
        public PlayerRun Run;
        public PlayerLedgeHang LedgeHang;
    }

}
