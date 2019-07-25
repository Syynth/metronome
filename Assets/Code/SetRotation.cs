using UnityEngine;
using System.Collections;

namespace Assets.Code
{

    public class SetRotation : MonoBehaviour
    {

        public Vector3 rotation = Vector3.zero;

        void LateUpdate()
        {
            var r = transform.rotation;
            r.eulerAngles = rotation;
            transform.rotation = r;
        }
    }


}