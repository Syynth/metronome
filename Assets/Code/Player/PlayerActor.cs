using Spine.Unity;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using Assets.Code.Interactive;
using Assets.Code.States;
using KinematicCharacterController;

namespace Assets.Code.Player
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputController))]
    [RequireComponent(typeof(RewiredInputSource))]
    [RequireComponent(typeof(PlayerStateMachineController))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    public class PlayerActor : MonoBehaviour, IStateMachine<PlayerActor>, IForceReceiver
    {
        public int frame = 0;

        public Vector3 velocity;
        public Vector3 gravity = Vector3.down * 10;
        public Vector2 input = Vector2.zero;
        public Vector2 aimInput = Vector2.zero;

        public List<Tuple<Collider, float>> ignoreColliders;
        public Animator animator;

        [SerializeField]
        private PlayerEditorStates PlayerEditorStates;

        public float duckJoystickThreshold = 0.25f;

        public LayerMask SolidLayer;
        public LayerMask OneWayLayer;

        public InputController Input;
        public KinematicCharacterMotor Motor;

        public HashSet<Collider> TouchingColliders = new HashSet<Collider>();

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

        public TState GetState<TState>() where TState : ActorState<PlayerActor>
        {
            return _states.FirstOrDefault(state => state.GetType() == typeof(TState)) as TState;
        }

        public void ChangeState<TState>() where TState : ActorState<PlayerActor>
        {
            var nextState = GetState<TState>();
            if (nextState == CurrentState) return;
            NextState = nextState;
            CurrentState.OnExit();
            PreviousState = CurrentState;
            CurrentState = nextState;
            NextState = null;
            CurrentState.OnEnter(Motor);
        }

        private HashSet<ActorState<PlayerActor>> _states = new HashSet<ActorState<PlayerActor>>();
        public ISet<ActorState<PlayerActor>> States => _states;

        #endregion

        void Start()
        {
            ignoreColliders = new List<Tuple<Collider, float>>();
            GetComponent<Rigidbody>().isKinematic = true;
            animator = GetComponentInChildren<Animator>();
            Motor = GetComponent<KinematicCharacterMotor>();
            Input = GetComponent<InputController>();
            
            new PlayerState[]
            {
                PlayerEditorStates.ClimbLadder,
                PlayerEditorStates.Duck,
                PlayerEditorStates.Fall,
                PlayerEditorStates.Idle,
                PlayerEditorStates.Jump,
                PlayerEditorStates.LedgeHang,
                PlayerEditorStates.Run,
            }.ToList().ForEach(state => _states.Add(state as ActorState<PlayerActor>));

            States.ToList().ForEach(state => state.SetActor(this));
            States.ToList().ForEach(state => state.OnStart());
            
            CurrentState = GetState<PlayerIdle>();
            PreviousState = GetState<PlayerIdle>();
        }

        void FixedUpdate()
        {
            frame += 1;
            States.ToList().ForEach(state => state.Tick());
            var skeleton = GetComponentInChildren<SkeletonAnimator>().skeleton;
            ignoreColliders = ignoreColliders.Where(pair => pair.Item2 > Time.time).ToList();
            var pos = GetState<PlayerFall>().LedgeDetect.transform.localPosition;
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
            GetState<PlayerFall>().LedgeDetect.transform.localPosition = pos;
            CurrentState.Render();
        }

        public void InputX()
        {
            bool downReleased = input.y >= -duckJoystickThreshold;
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            GetState<PlayerRun>().xPressed = input.x != 0;
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

            var run = GetState<PlayerRun>();
            var jump = GetState<PlayerJump>();
            var duck = GetState<PlayerDuck>();

            run.down = Input.GetButton("Run");
            
            var held = jump.held;
            jump.held = Input.GetButton("Jump");
            jump.pressed = jump.held && !held;
            duck.pressed = input.y < -duckJoystickThreshold && downReleased;
            duck.held = input.y < -duckJoystickThreshold;
            var sign = Math.Sign(input.x);
            if (sign == Direction.Right)
            {
                run.vMax = run.maxSpeed;
            }
            else if (sign == Direction.Left)
            {
                run.vMax = -run.maxSpeed;
            }
        }

        public void AccelerateX()
        {
            var run = GetState<PlayerRun>();
            var runSpeed = run.down ? run.maxSpeed : run.maxSpeed * run.walkThreshold;
            var maxSpeed = aiming ? run.maxSpeed * 0.2f : runSpeed;
            var dx = run.acceleration * Mathf.Sign(run.vMax) * Time.deltaTime;
            if (run.xPressed)
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
                    velocity.x = maxSpeed * Mathf.Sign(run.vMax);
                }
            }
            if (Mathf.Abs(velocity.x) < run.friction * Time.deltaTime)
            {
                velocity.x = 0;
            }
            else
            {
                velocity.x -= run.friction * Mathf.Sign(velocity.x) * Time.deltaTime;
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
            TouchingColliders.Add(collider);
            Debug.Log("collided with trigger");
            var door = collider.gameObject.GetComponent<SceneEntrance>();
            if (door != null && door.TransitionImmediately)
            {
                door.Enter();
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            TouchingColliders.Remove(collider);
        }

    }

    [Serializable]
    public struct PlayerEditorStates
    {
        public PlayerDuck Duck;
        public PlayerFall Fall;
        public PlayerIdle Idle;
        public PlayerJump Jump;
        public PlayerLedgeHang LedgeHang;
        public PlayerClimbLadder ClimbLadder;
        public PlayerRun Run;
    }

}
