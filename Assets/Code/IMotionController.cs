using UnityEngine;

namespace Assets.Code
{
    public interface IMotionController
    {

        CollisionInfo Move(Vector3 velocity, Vector3 down);
        CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position);

        bool OnJumpThrough(Vector3 down);
    }
}
