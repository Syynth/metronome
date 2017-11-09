using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code
{
    public interface IMotionController
    {

        LayerMask SolidLayer { get; }
        LayerMask JumpThroughLayer { get; }
        LayerMask AllLayer { get; }

        CollisionInfo Move(Vector3 velocity, Vector3 down, List<Collider> ignore, bool findGround = false);
        CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position);

        bool OnJumpThrough(Vector3 down, out Collider collider);
        bool CanStand(Vector3 normal);
    }

    public struct CollisionInfo
    {
        public bool Below { get; set; }
        public bool Above { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
    }

}
