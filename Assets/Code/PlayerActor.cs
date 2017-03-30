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

        MotionController motionController;

        void Start()
        {
            motionController = GetComponent<MotionController>();
        }

        void Update()
        {
            velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * Time.deltaTime * moveSpeed;
            motionController.Move(velocity);
        }

    }

}
