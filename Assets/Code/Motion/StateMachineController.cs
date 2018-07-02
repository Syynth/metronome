using UnityEngine;
using System.Collections;
using Assets.Code.States;
using KinematicCharacterController;

namespace Assets.Code.Motion
{

    [RequireComponent(typeof(InputController))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    [RequireComponent(typeof(Collider))]
    public class StateMachineController<T> : BaseCharacterController where T : MonoBehaviour, IStateMachine<T>
    {

        IStateMachine<T> Actor { get; set; }

        public LayerMask SolidLayer;
        public LayerMask OneWayLayer;

        private CapsuleCollider Collider;

        private void Start()
        {
            Actor = GetComponent<IStateMachine<T>>();
            Collider = GetComponent<CapsuleCollider>();
        }

        public override void BeforeCharacterUpdate(float deltaTime) {
            Actor.CurrentState.BeforeUpdate(deltaTime, Motor);
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            Actor.CurrentState.UpdateVelocity(ref currentVelocity, Motor);
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }

        public override bool IsColliderValidForCollisions(Collider coll)
        {
            return Utils.IsInLayerMask(coll.gameObject.layer, SolidLayer | OneWayLayer);
        }

        public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

        public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

        public override void PostGroundingUpdate(float deltaTime) { }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            Actor.CurrentState.AfterUpdate(deltaTime, Motor);
        }
        
    }


}