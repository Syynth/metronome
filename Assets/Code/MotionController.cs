using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(BoxCollider2D))]
    class MotionController : RaycastController
    {

        public void Move(Vector3 velocity)
        {
            var hit = BoxCast(boxCollider.bounds, velocity, velocity.magnitude, solidLayer);
            if (hit)
            {
                transform.position += velocity.normalized * (hit.distance - skinWidth);
                return;
            }
            transform.position += velocity;
        }

    }

}
