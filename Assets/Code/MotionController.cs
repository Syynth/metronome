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
                var travel = velocity.normalized * (hit.distance - skinWidth);
                transform.position += travel;
                var rem = velocity - travel;
                var rot = new Vector2(-hit.normal.y, hit.normal.x).normalized;
                Move(rot * Vector3.Dot(rem, rot));
                return;
            }
            transform.position += velocity;
        }

    }

}
