﻿using Spine.Unity;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using Assets.Code.Interactive;

namespace Assets.Code.Player
{

    [RequireComponent(typeof(IMotionController))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputController))]
    [RequireComponent(typeof(RewiredInputSource))]
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerActor : MonoBehaviour, IStateMachine<PlayerActor>, IForceReceiver
    {
        public int frame = 0;

        public Vector3 velocity;
        public Vector3 gravity = Vector3.down * 10;
        public Vector2 input = Vector2.zero;
        public Vector2 aimInput = Vector2.zero;

        public IMotionController motionController;

        public PlayerStates states;
        public List<Tuple<Collider, float>> ignoreColliders;
        public Animator animator;

        public float duckJoystickThreshold = 0.25f;

        public InputController Input;

        public bool aiming = false;

        #region IForceReceiver Implementation

        public void ReceiveForce(Vector3 force, bool doubleOpposingForce)
        {
            velocity += force;
            if (Vector3.Dot(velocity, force) < 0)
            {
                velocity += force;
            }
        }

        #endregion

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

        public List<ActorState<PlayerActor>> States
        {
            get
            {
                return new List<ActorState<PlayerActor>>(
                    new ActorState<PlayerActor>[] {
                        states.Run,
                        states.Jump,
                        states.Idle,
                        states.Fall,
                        states.Duck,
                        states.LedgeHang
                    }
                );
            }
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
            Input = GetComponent<InputController>();

            States.ForEach(state => state.SetActor(this));
            States.ForEach(state => state.OnStart());
            
            CurrentState = states.Idle;
            PreviousState = states.Idle;

        }

        void FixedUpdate()
        {
            frame += 1;
            States.ForEach(state => state.Tick());
            if (motionController.Initialized)
            {
                CurrentState.Update();
            }
            var skeleton = GetComponentInChildren<SkeletonAnimator>().skeleton;
            ignoreColliders = ignoreColliders.Where(pair => pair.Item2 > Time.time).ToList();
            var pos = states.Fall.LedgeDetect.transform.localPosition;
            if (velocity.x < 0)
            {
                skeleton.flipX = true;
                pos.x = -0.7f;
            }
            if (velocity.x > 0)
            {
                skeleton.flipX = false;
                pos.x = 0.7f;
            }
            states.Fall.LedgeDetect.transform.localPosition = pos;
            CurrentState.Render();
        }

        public void InputX()
        {
            bool downReleased = input.y >= -duckJoystickThreshold;
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            states.Run.xPressed = input.x != 0;
            aiming = Input.GetButton("Aim");
            if (aiming)
            {
                if (Input.GetButton("MouseAim"))
                {
                    var pos = transform.position;
                    pos.y += 3;
                    Vector2 p = Camera.main.WorldToScreenPoint(pos);
                    var m = Input.mousePosition;
                    aimInput = (m - p).normalized;
                }
                else
                {
                    aimInput = Input.GetAxis2DRaw("AimX", "AimY");
                }
            }
            else
            {
                aimInput = Vector2.zero;
            }

            states.Run.down = Input.GetButton("Run");
            
            var held = states.Jump.held;
            states.Jump.held = Input.GetButton("Jump");
            states.Jump.pressed = states.Jump.held && !held;
            states.Duck.pressed = input.y < -duckJoystickThreshold && downReleased;
            states.Duck.held = input.y < -duckJoystickThreshold;
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
            var runSpeed = states.Run.down ? states.Run.maxSpeed : states.Run.maxSpeed * states.Run.walkThreshold;
            var maxSpeed = aiming ? states.Run.maxSpeed * 0.2f : runSpeed;
            var dx = states.Run.acceleration * Mathf.Sign(states.Run.vMax) * Time.deltaTime;
            if (states.Run.xPressed)
            {
                // Accumulate speed when traveling
                velocity.x += dx;

                // Bonus speed if you're going slow
                if (Mathf.Abs(velocity.x) < maxSpeed * 0.5f)
                {
                    velocity.x += dx;
                }

                if (Mathf.Abs(velocity.x) > maxSpeed)
                {
                    velocity.x = maxSpeed * Mathf.Sign(states.Run.vMax);
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

        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log("collided with trigger");
            var door = collider.gameObject.GetComponent<SceneEntrance>();
            if (door != null && door.TransitionImmediately)
            {
                door.Enter();
            }
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
