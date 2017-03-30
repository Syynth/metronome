using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(BoxCollider2D))]
    class RaycastController : MonoBehaviour
    {

        public const float skinWidth = 0.01f;

        BoxCollider2D boxCollider;

        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

    }


    struct RaycastInfo
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

}
