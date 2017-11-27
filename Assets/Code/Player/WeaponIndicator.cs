using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Player
{

    [RequireComponent(typeof(MeshRenderer))]
    public class WeaponIndicator : MonoBehaviour
    {

        PlayerActor actor;
        MeshRenderer meshRenderer;

        public float radius = 2f;

        void Start()
        {
            actor = transform.parent.GetComponent<PlayerActor>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void Update()
        {
            if (actor.aiming != meshRenderer.enabled)
            {
                meshRenderer.enabled = actor.aiming;
            }
            var p = actor.aimInput * radius;
            p.y += 3;
            transform.localPosition = p;
        }
    }


}
