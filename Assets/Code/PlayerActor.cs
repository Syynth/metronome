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

        public Vector3 velocity;
        MotionController motionController;

        void Start()
        {
            motionController = GetComponent<MotionController>();
        }

        void Update()
        {
            motionController.Move(velocity);
        }

    }

}
