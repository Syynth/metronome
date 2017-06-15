using UnityEngine;

namespace Assets.Code
{
    public interface IMotionController
    {

        LayerMask SolidLayer { get; }
        LayerMask JumpThroughLayer { get; }
        LayerMask AllLayer { get; }

        CollisionInfo Move(Vector3 velocity, Vector3 down);
        CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position);

        bool OnJumpThrough(Vector3 down);
        bool CanStand(Vector2 normal);
    }
}
