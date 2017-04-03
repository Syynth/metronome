using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(MotionController))]
    class PlayerActor : MonoBehaviour
    {

        public float moveSpeed = 10f;
        public Vector3 velocity;
        public Vector3 gravity = Vector3.down * 10;

        MotionController motionController;

        void Start()
        {
            motionController = GetComponent<MotionController>();
        }

        void UpdateVelocity()
        {
            velocity += gravity * Time.deltaTime;
            velocity.x = Input.GetAxisRaw("Horizontal");
            velocity.y = Mathf.Max(-10f, velocity.y);
        }

        void Update()
        {
            UpdateVelocity();
            var info = motionController.Move(velocity * Time.deltaTime, Vector3.down);
            if (info.Below)
            {
                velocity.y = 0;
            }
        }

    }

}
