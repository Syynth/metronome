using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code
{
    public interface IMotionController
    {

        LayerMask SolidLayer { get; }
        LayerMask JumpThroughLayer { get; }
        LayerMask AllLayer { get; }

        CollisionInfo Move(Vector3 velocity, Vector3 down, List<Collider2D> ignore);
        CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position);

        bool OnJumpThrough(Vector3 down, out Collider2D collider);
        bool CanStand(Vector2 normal);
    }
}
