using System;
using UnityEngine;    

namespace Assets.Code
{
    [RequireComponent(typeof(Camera))]
    class SmoothCameraFollow : MonoBehaviour
    {

        public PlayerActor target;

        [Range(0, 0.9f)]
        public float horizontalLag = 1f / 9f;
        
        private float age;
        public float horizontalLagTime = 1.5f;

        [Range(0, 1)]
        public float minimumHorizontalLead = 0.4f;
        [Range(0, 1)]
        public float maxHorizontalLead = 0.6f;

        [Range(0, 0.9f)]
        public float verticalLag = 0.5f;

        [Range(0, 0.5f)]
        public float maxVerticalLead = 0.3f;

        private float dx, dy;

        private Camera camera;

        void Start()
        {
            dx = target.transform.position.x;
            dy = target.transform.position.y;
            camera = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            var percentSpeed = Mathf.Abs(target.velocity.x) / target.maxSpeed.x;
            var percentAge = horizontalLagTime != 0 ? age / horizontalLagTime : 0;

            if (percentSpeed > 0.5f)
            {
                age += Time.deltaTime;
            }
            else
            {
                age = 0;
            }

            var p0 = camera.ViewportToWorldPoint(Vector3.zero);
            var p1 = camera.ViewportToWorldPoint(Vector3.one);

            var rect = new Rect(p0.x, p1.y, p1.x - p0.x, p1.y - p0.y);

            float gx = target.transform.position.x + (rect.width * minimumHorizontalLead) * Mathf.Sign(target.maxSpeed.x) * Mathf.Min(1, percentSpeed * Mathf.Min(percentAge, 1));
            float gy = target.transform.position.y;

            float xFac = 1f / horizontalLag;
            float yFac = 1f / verticalLag;

            if (Mathf.Abs(transform.position.x - gx) > rect.width * maxHorizontalLead)
            {
                dx -= (dx - gx) / (xFac / 3f);
            }
            else
            {
                dx -= (dx - gx) / xFac;
            }

            if (Mathf.Abs(transform.position.y - gy) > rect.height * maxVerticalLead)
            {
                dy -= (dy - gy) / (yFac / 3);
            }
            else
            {
                dy -= (dy - gy) / yFac;
            }

            var pos = transform.position;

            pos.x -= (pos.x - dx) / (xFac / 2f);
            pos.y -= (pos.y - dy) / yFac;

            transform.position = pos;

        }

    }
}
