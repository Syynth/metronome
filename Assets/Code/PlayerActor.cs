using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(MotionController))]
    [RequireComponent(typeof(Rigidbody2D))]
    class PlayerActor : MonoBehaviour
    {

        public Vector2 moveSpeed = new Vector2(10f, 25f);
        public Vector3 velocity;
        public Vector3 gravity = Vector3.down * 10;

        public Vector2 maxSpeed = new Vector2(10f, 25f);

        MotionController motionController;

        void Start()
        {
            motionController = GetComponent<MotionController>();
            GetComponent<Rigidbody2D>().isKinematic = true;
        }

        void UpdateVelocity()
        {
            velocity += gravity * Time.deltaTime;
            velocity.x = Input.GetAxisRaw("Horizontal") * moveSpeed.x;
            velocity.y = Mathf.Max(-100f, velocity.y);
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = moveSpeed.y;
            }
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
