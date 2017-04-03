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

        Vector3 GetMoveVector(Vector3 normal, Vector3 velocity)
        {
            var g1 = new Vector3(-normal.y, normal.x);
            var g2 = g1 * -1;
            return (Vector3.Dot(velocity, g1) >= 0 ? g1 : g2).normalized * velocity.magnitude;
        }

        CollisionInfo Travel(Vector3 velocity, Vector3 down)
        {
            var hit = BoxCast(boxCollider.bounds, velocity, velocity.magnitude, solidLayer);
            if (hit)
            {
                var travel = velocity.normalized * (hit.distance - skinWidth);
                transform.position += travel;
                var rem = velocity - travel;
                var rot = new Vector2(-hit.normal.y, hit.normal.x).normalized;
                return Move(rot * Vector3.Dot(rem, rot), down);
            }
            transform.position += velocity;
            return new CollisionInfo();
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            RaycastHit2D hit;
            hit = BoxCast(boxCollider.bounds, down, skinWidth * 2, solidLayer);
            if (hit && Vector3.Dot(velocity, down) > 0)
            {
                var info = new CollisionInfo()
                {
                    Below = true
                };
                if (Vector3.Distance(velocity.normalized, down.normalized) < 0.01f)
                {
                    return info;
                }
                var move = GetMoveVector(hit.normal, velocity);
                return info.Or(Travel(move, down));
            }
            return Travel(velocity, down);
        }

    }

    struct CollisionInfo
    {
        public bool Below { get; set; }
        public bool Above { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public CollisionInfo Or(CollisionInfo info)
        {
            return new CollisionInfo()
            {
                Below = info.Below || Below,
                Above = info.Above || Above,
                Left = info.Left || Left,
                Right = info.Right || Right,
            };
        }
    }

}
