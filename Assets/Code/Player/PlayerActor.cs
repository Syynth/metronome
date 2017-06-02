using Spine.Unity;
using System;
using UnityEngine;

namespace Assets.Code.Player
{

    [RequireComponent(typeof(IMotionController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerActor : MonoBehaviour, IStateMachine<PlayerActor>
    {

        public Vector3 velocity;
        public Vector3 gravity = Vector3.down * 10;
        public Vector2 input = Vector2.zero;

        IMotionController motionController;

        public PlayerStates states;

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
            motionController = GetComponent<IMotionController>();
            GetComponent<Rigidbody>().isKinematic = true;
            states.Duck.SetActor(this);
            states.Fall.SetActor(this);
            states.Idle.SetActor(this);
            states.Jump.SetActor(this);
            states.Run.SetActor(this);
            CurrentState = states.Idle;
            PreviousState = states.Idle;
            CurrentState.OnEnter();
        }

        void Update()
        {
            CurrentState.Update();
            CurrentState.Render();
            var skeleton = GetComponent<SkeletonAnimation>().skeleton;
            if (velocity.x < 0)
            {
                skeleton.flipX = true;
            }
            if (velocity.x > 0)
            {
                skeleton.flipX = false;
            }
        }

        public void InputX()
        {
            bool downReleased = input.y >= 0;
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            states.Run.pressed = input.x != 0;
            states.Jump.pressed = Input.GetButtonDown("Jump");
            states.Jump.held = Input.GetButton("Jump");
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

        public CollisionInfo Move(Vector3 velocity)
        {
            return motionController.Move(velocity, gravity);
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
    }

}
