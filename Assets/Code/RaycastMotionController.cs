using UnityEngine;
using System.Collections;
using System;

namespace Assets.Code
{
    
    class RaycastMotionController : RaycastController, IMotionController
    {

        public LayerMask SolidLayer => solidLayer;
        public LayerMask JumpThroughLayer => oneWayLayer;
        public LayerMask AllLayer => solidLayer | oneWayLayer;

        RaycastInfo info;

        public CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position)
        {
            return new CollisionInfo();
        }

        bool OnGround(out RaycastHit2D hit)
        {
            hit = new RaycastHit2D();
            return false;
        }

        Vector2 GetMoveVector(Vector2 normal)
        {
            return Vector2.zero;
        }

        void MoveX(float x)
        {
            if (x == 0) return;
            var pos = transform.position;
            var hit = RaycastLine(x > 0 ? info.topLeft : info.topRight, x > 0 ? info.bottomLeft : info.bottomRight, Vector2.right * (x > 0 ? 1 : -1), x, null);
            if (hit)
            {
                pos.x += hit.distance;
            }
            else
            {
                pos.x += x;
            }
            transform.position = pos;
        }

        void MoveY(float y)
        {
            if (y == 0) return;
            var pos = transform.position;
            var hit = RaycastLine(y > 0 ? info.topLeft : info.bottomLeft, y > 0 ? info.topRight : info.bottomRight, Vector2.right * (y > 0 ? 1 : -1), y, null);
            if (hit)
            {
                pos.y += hit.distance;
            }
            else
            {
                pos.y += y;
            }
            transform.position = pos;
        }

        void Recalculate()
        {
            info.topLeft = new Vector2(boxCollider.bounds.min.x + skinWidth, boxCollider.bounds.max.y - skinWidth);
            info.topRight = new Vector2(boxCollider.bounds.max.x - skinWidth, boxCollider.bounds.max.y - skinWidth);
            info.bottomLeft = new Vector2(boxCollider.bounds.min.x + skinWidth, boxCollider.bounds.min.y + skinWidth);
            info.bottomRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            info = new RaycastInfo();
            if (OnGround(out var hit))
            {
                var move = GetMoveVector(hit.normal);
                if (move.y > 0)
                {
                    MoveY(velocity.y);
                    MoveX(velocity.x);
                }
                else
                {
                    MoveX(velocity.x);
                    MoveY(velocity.y);
                }
            }
            else
            {
                MoveX(velocity.x);
                MoveY(velocity.y);
            }
            return new CollisionInfo();
        }

        public bool OnJumpThrough(Vector3 down)
        {
            return false;
        }
    }

    struct RaycastInfo
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }


}