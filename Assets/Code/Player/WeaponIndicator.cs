using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Player
{

    [RequireComponent(typeof(Renderer))]
    public class WeaponIndicator : MonoBehaviour
    {

        PlayerActor actor;
        Renderer renderer;

        public float radius = 2f;
        public float yOffset = 3.5f;

        public Transform Lazer;
        public LineRenderer Line;

        void Start()
        {
            actor = transform.parent.GetComponent<PlayerActor>();
            renderer = GetComponent<Renderer>();
            if (Lazer == null)
            {
                Lazer = transform.Find("Lazer");
            }
            if (Line == null)
            {
                Line = transform.Find("Line").GetComponent<LineRenderer>();
            }
        }

        void Update()
        {
            if (actor.aimInput == Vector2.zero)
            {
                renderer.enabled = false;
                Lazer.gameObject.SetActive(false);
                Line.enabled = false;
            }
            else if (actor.aiming)
            {
                renderer.enabled = true;
                Lazer.gameObject.SetActive(true);
                Line.enabled = true;
            }

            var r = 5000f;

            transform.localPosition = new Vector3(0, yOffset, transform.localPosition.z);

            var m = renderer.material;
            var c = m.color;
            c.a = 1f;
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, actor.aimInput.normalized), out hit, Mathf.Infinity, actor.SolidLayer))
            {
                r = hit.distance;
                if (hit.distance < radius)
                {
                    c.a = 0.3f;
                    Line.enabled = false;
                }   
            }
            m.color = c;
            renderer.material = m;

            Vector3 p = actor.aimInput.normalized * r;
            p.y += yOffset;
            var e = new Vector3();
            var a = Vector3.Angle(Vector3.down, transform.localPosition - p);
            if (Vector3.Dot(transform.localPosition - p, Vector3.right) < 0)
            {
                a = 360 - a;
            }
            e.z = a;
            transform.localEulerAngles = e;
            transform.localPosition = p;
            var pp = transform.position;
            var pl = actor.aimInput.normalized * radius;
            pl.y += yOffset;
            transform.localPosition = pl;
            Lazer.position = pp;
            Line.SetPositions(new Vector3[]
            {
                transform.position,
                Lazer.position,
            });
            
        }
    }


}
