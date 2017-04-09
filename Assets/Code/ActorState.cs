using UnityEngine;
using System.Collections;
using System;


namespace Assets.Code
{

    [Serializable]
    public abstract class ActorState<T> where T : MonoBehaviour, IStateMachine<T>
    {
        [SerializeField]
        private bool Active;

        public float Age = 0;
        public SpriteAnimation animation;

        protected T actor;
        protected CollisionInfo info;
        
        public virtual void SetActor(T actor)
        {
            this.actor = actor;
            if (actor.GetComponent<SpriteRenderer>() == null)
            {
                throw new ArgumentException("Actor must contain a SpriteRenderer component");
            }
            if (actor.GetComponent<MotionController>() == null)
            {
                throw new ArgumentException("Actor must contain a MotionController component");
            }
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
            actor.GetComponent<SpriteRenderer>().sprite = animation.getFrame(Age);
        }

        
    }


}
