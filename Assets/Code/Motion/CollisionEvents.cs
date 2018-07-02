using UnityEngine;
using System.Collections;

namespace Assets.Code
{


    public class CollisionEvents : MonoBehaviour
    {

        public delegate void CollisionHandler(Collision2D collider);
        public event CollisionHandler OnCollisionEnter;
        public event CollisionHandler OnCollisionStay;
        public event CollisionHandler OnCollisionExit;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollisionEnter?.Invoke(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnCollisionStay?.Invoke(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            OnCollisionExit?.Invoke(collision);
        }

    }


}