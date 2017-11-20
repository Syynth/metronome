using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{

    public interface IInputSource
    {

        bool GetButton(string button);
        bool GetButtonDown(string button);
        float GetAxis(string axis);
        Vector2 GetAxis2D(string xAxis, string yAxis);

    }

    public class InputController : MonoBehaviour
    {

        Stack<IInputSource> InputSources = new Stack<IInputSource>();

        // Use this for initialization
        void Start()
        {
            InputSources.Push(GetComponent<IInputSource>());
        }

        public bool GetButton(string button)
        {
            return InputSources.Peek().GetButton(button);
        }

        public bool GetButtonDown(string button)
        {
            return InputSources.Peek().GetButtonDown(button);
        }

        public float GetAxis(string axis)
        {
            return InputSources.Peek().GetAxis(axis);
        }

        public Vector2 GetAxis2D(string xAxis, string yAxis)
        {
            return InputSources.Peek().GetAxis2D(xAxis, yAxis);
        }

    }


}