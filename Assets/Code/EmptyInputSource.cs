using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public class EmptyInputSource : MonoBehaviour, IInputSource
    {

        public float GetAxis(string axis)
        {
            return 0;
        }

        public Vector2 GetAxis2D(string xAxis, string yAxis)
        {
            return Vector2.zero;
        }

        public float GetAxisRaw(string axis)
        {
            return 0;
        }

        public Vector2 GetAxis2DRaw(string xAxis, string yAxis)
        {
            return Vector2.zero;
        }

        public bool GetButton(string button)
        {
            return false;
        }

        public bool GetButtonDown(string button)
        {
            return false;
        }

        public Vector2 mousePosition => Vector2.zero;

    }
}
