using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(BoxCollider))]
    class RaycastController : MonoBehaviour
    {

        public const float skinWidth = 0.05f;
        public LayerMask solidLayer;
        public LayerMask oneWayLayer;

        protected BoxCollider boxCollider;

        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        void DrawX(Vector2 point, float xSize, Color color)
        {
            Debug.DrawLine(new Vector2(point.x - xSize, point.y - xSize), new Vector2(point.x + xSize, point.y + xSize), color);
            Debug.DrawLine(new Vector2(point.x + xSize, point.y - xSize), new Vector2(point.x - xSize, point.y + xSize), color);
        }

        protected virtual RaycastHit BoxCast(Bounds bounds, Vector3 direction, float distance, LayerMask layer)
        {
            var hits = Physics.BoxCastAll(bounds.center, bounds.size / 2, direction, Quaternion.identity, distance, layer);
            var hit = hits.OrderBy(h => h.distance).FirstOrDefault();
            Color col = hit.IsValid() ? Color.red : Color.white;
            Color col2 = hit.IsValid() ? Color.green : Color.blue;
            var min = bounds.min + direction;
            var max = bounds.max + direction;
            Debug.DrawLine(bounds.min, new Vector2(bounds.min.x, bounds.max.y), col);
            Debug.DrawLine(bounds.min, new Vector2(bounds.max.x, bounds.min.y), col);
            Debug.DrawLine(bounds.max, new Vector2(bounds.min.x, bounds.max.y), col);
            Debug.DrawLine(bounds.max, new Vector2(bounds.max.x, bounds.min.y), col);
            Debug.DrawLine(min, new Vector2(min.x, max.y), col2);
            Debug.DrawLine(min, new Vector2(max.x, min.y), col2);
            Debug.DrawLine(max, new Vector2(min.x, max.y), col2);
            Debug.DrawLine(max, new Vector2(max.x, min.y), col2);
            if (hit.IsValid())
            {
                DrawX(hit.point, .15f, Color.cyan);
            }
            return hit;

        }

        protected virtual Collider BoxCheck(Bounds bounds, LayerMask layer)
        {
            bounds.Expand(-skinWidth * 2);
            return Physics.OverlapBox(bounds.center, bounds.size / 2, Quaternion.identity, layer).FirstOrDefault();
        }

    }

}
