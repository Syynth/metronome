using UnityEngine;
using System.Collections;
using System;
using Spine.Unity;
using Assets.Code.Player;

using KinematicCharacterController;

namespace Assets.Code.States
{

    [Serializable]
    public abstract class ActorState<T> where T : MonoBehaviour, IStateMachine<T>
    {
        [SerializeField]
        private bool Active;

        public float Age = 0;

        [SerializeField]
        public string AnimationName;

        protected T Actor;
        protected IInputSource Input;
        
        public virtual void SetActor(T actor)
        {
            Actor = actor;
            Input = Actor.GetComponent<IInputSource>();
        }

        public virtual void OnEnter(KinematicCharacterMotor motor)
        {
            Age = 0;
            Active = true;
        }

        public virtual void BeforeUpdate(float deltaTime, KinematicCharacterMotor motor)
        {
            Age += Time.deltaTime;
        }

        public virtual void UpdateVelocity(ref Vector3 velocity, float deltaTime, KinematicCharacterMotor motor) { }

        public virtual void UpdateRotation(ref Quaternion rotation, float deltaTime, KinematicCharacterMotor motor) { }

        public virtual void OnExit()
        {
            Active = false;
        }

        public virtual void AfterUpdate(float deltaTime, KinematicCharacterMotor motor) { }

        public virtual void OnStart() { }

        public virtual void Tick() { }

        public virtual void Render() { }

        
    }


}
