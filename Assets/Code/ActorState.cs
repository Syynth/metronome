using UnityEngine;
using System.Collections;
using System;
using Spine.Unity;

namespace Assets.Code
{

    [Serializable]
    public abstract class ActorState<T> where T : MonoBehaviour, IStateMachine<T>
    {
        [SerializeField]
        private bool Active;

        public float Age = 0;

        [SerializeField]
        public string AnimationName;
        
        protected T actor;
        protected CollisionInfo info;
        
        public virtual void SetActor(T actor)
        {
            this.actor = actor;
            //if (actor.GetComponent<SkeletonAnimation>() == null)
            //{
            //    throw new ArgumentException("Actor must contain a SkeletonAnimation component");
            //}
        }

        public virtual void OnStart()
        {

        }

        public virtual void OnEnter()
        {
            Age = 0;
            Active = true;
        }

        public virtual void OnExit()
        {
            Active = false;
        }

        public virtual void Update()
        {
            Age += Time.deltaTime;
        }

        public virtual void Render()
        {
            //actor.GetComponent<SkeletonAnimation>().AnimationName = AnimationName;
        }

        
    }


}
