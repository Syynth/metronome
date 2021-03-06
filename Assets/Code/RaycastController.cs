﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(BoxCollider2D))]
    class RaycastController : MonoBehaviour
    {

        public const float skinWidth = 0.1f;
        public float maxRaySeparation = 0.2f;
        public LayerMask solidLayer;
        public LayerMask oneWayLayer;

        [HideInInspector]
        public int horizontalRayCount;
        [HideInInspector]
        public int verticalRayCount;

        [HideInInspector]
        public float horizontalRaySpacing;
        [HideInInspector]
        public float verticalRaySpacing;

        protected BoxCollider2D boxCollider;

        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            CalculateRaySpacing();
        }

        protected virtual void CalculateRaySpacing()
        {
            Bounds bounds = boxCollider.bounds;
            bounds.Expand(skinWidth * -2);

            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            horizontalRayCount = Mathf.RoundToInt(boundsHeight / maxRaySeparation);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / maxRaySeparation);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        void DrawX(Vector2 point, float xSize, Color color)
        {
            Debug.DrawLine(new Vector2(point.x - xSize, point.y - xSize), new Vector2(point.x + xSize, point.y + xSize), color);
            Debug.DrawLine(new Vector2(point.x + xSize, point.y - xSize), new Vector2(point.x - xSize, point.y + xSize), color);
        }

        protected virtual RaycastHit2D RaycastLine(Vector2 pointA, Vector2 pointB, Vector2 direction, float distance, IEnumerable<Collider2D> ignore)
        {
            var empty = new Collider2D[0];
            List<RaycastHit2D[]> hits = new List<RaycastHit2D[]>();
            float rayCount = Mathf.Ceil(Vector2.Distance(pointA, pointB) / maxRaySeparation);
            var segment = pointB - pointA;
            for (float i = 0; i <= rayCount; ++i)
            {
                hits.Add(Physics2D.RaycastAll(pointA + segment * (i / rayCount) - direction.normalized * skinWidth, direction, distance + skinWidth * 2, solidLayer));
                //Debug.DrawRay(pointA + segment * (i / rayCount), direction.normalized * distance);
            }
            var ret = hits.SelectMany(l => l).Where(hit => hit.collider != null && !(ignore ?? empty).Contains(hit.collider)).OrderBy(hit => hit.distance).FirstOrDefault();
            DrawX(ret.point, .1f, Color.grey);
            Debug.DrawLine(ret.point, ret.point + ret.normal * .2f, Color.red);
            return ret;
        }

        protected virtual RaycastHit2D BoxCastUnstable(Bounds bounds, Vector3 direction, float distance, LayerMask layer, IEnumerable<Collider2D> ignore = null)
        {
            float vd = direction.y >= 0 ? 1 : -1;
            float hd = direction.x >= 0 ? 1 : -1;
            float hy = vd >= 0 ? bounds.min.y + skinWidth : bounds.max.y - skinWidth;
            float vx = bounds.center.x + (hd * bounds.size.x / 2) - (hd * skinWidth);
            var hPoint1 = new Vector2(bounds.min.x + skinWidth, hy);
            var hPoint2 = new Vector2(bounds.max.x - skinWidth, hy);
            var vPoint1 = new Vector2(vx, hy + bounds.size.y * -vd - skinWidth);
            var sideHit = direction.x != 0 ? RaycastLine(hd < 0 ? hPoint1 : hPoint2, vPoint1, direction, distance, ignore ?? new Collider2D[0]) : new RaycastHit2D();
            var vertHit = direction.y != 0 ? RaycastLine(hPoint1, hPoint2, direction, distance, ignore ?? new Collider2D[0]) : new RaycastHit2D();
            if (sideHit && vertHit)
            {
                return sideHit.distance < vertHit.distance ? sideHit : vertHit;
            }
            return sideHit ? sideHit : vertHit;
        }

        protected virtual RaycastHit2D BoxCast(Bounds bounds, Vector3 direction, float distance, LayerMask layer)
        {
            var hits = Physics2D.BoxCastAll(bounds.center, bounds.size, 0, direction, distance + skinWidth, layer);
            var hit = hits.OrderBy(h => h.distance).FirstOrDefault();
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
