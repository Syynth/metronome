using UnityEngine;
using System.Collections;
using System;
using Assets.Code.Player;

namespace Assets.Code.Interactive
{

    public class Fan : MonoBehaviour
    {

        public float Force = 20;
        new Rigidbody rigidbody;
        new Collider collider;
        MeshFilter filter;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            filter = GetComponent<MeshFilter>();
        }

        private void Update()
        {
            rigidbody.WakeUp();
        }

        void OnCollisionStay(Collision collider)
        {
            var f = collider.gameObject.GetComponent<IForceReceiver>();
            if (f != null)
            {
                var height = filter.mesh.bounds.size.y;
                var up = transform.up * Force;
                var pos = collider.gameObject.transform.position;
                var distance = pos - transform.position;
                var projected = Vector3.Project(distance, transform.up * height);
                var force = Vector3.Lerp(up, up / 3, (height - distance.magnitude) / height);
                f.ReceiveForce(force * Time.deltaTime, true);
            }
        }

    }
	
}
