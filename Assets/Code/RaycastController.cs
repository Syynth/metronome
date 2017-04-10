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

        public const float skinWidth = 0.025f;
        public LayerMask solidLayer;
        public LayerMask oneWayLayer;

        protected BoxCollider2D boxCollider;

        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        void DrawX(Vector2 point, float xSize, Color color)
        {
            Debug.DrawLine(new Vector2(point.x - xSize, point.y - xSize), new Vector2(point.x + xSize, point.y + xSize), color);
            Debug.DrawLine(new Vector2(point.x + xSize, point.y - xSize), new Vector2(point.x - xSize, point.y + xSize), color);
        }

        protected virtual RaycastHit2D BoxCast(Bounds bounds, Vector3 direction, float distance, LayerMask layer)
        {
            //bounds.Expand(new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y)).normalized * -skinWidth);
            RaycastHit2D hit = new RaycastHit2D();
            hit = Physics2D.BoxCast(bounds.center, bounds.size, 0, direction, distance + skinWidth, layer);
            Color col = hit ? Color.red : Color.white;
            Color col2 = hit ? Color.green : Color.blue;
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
            if (hit)
            {
                DrawX(hit.point, .15f, Color.cyan);
                DrawX(hit.centroid, .15f, Color.black);
            }
            return hit;
        }

        protected virtual Collider2D BoxCheck(Bounds bounds, LayerMask layer)
        {
            bounds.Expand(-skinWidth * 2);
            return Physics2D.OverlapBox(bounds.center, bounds.size, 0, layer);
        }

    }

}
