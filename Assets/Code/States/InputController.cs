using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.States
{

    public interface IInputSource
    {

        bool GetButton(string button);
        bool GetButtonDown(string button);
        float GetAxis(string axis);
        float GetAxisRaw(string axis);
        Vector2 GetAxis2D(string xAxis, string yAxis);
        Vector2 GetAxis2DRaw(string xAxis, string yAxis);
        Vector2 mousePosition { get; }

    }

    [RequireComponent(typeof(IInputSource))]
    public class InputController : MonoBehaviour
    {

        Stack<IInputSource> InputSources = new Stack<IInputSource>();

        void Start()
        {
            InputSources.Push(GetComponent<IInputSource>());
        }

        public void PushInputSource(IInputSource inputSource)
        {
            InputSources.Push(inputSource);
        }

        public IInputSource GetCurrentInputSource()
        {
            return InputSources.Peek();
        }

        public IInputSource ReleaseInputSource()
        {
            if (InputSources.Count > 1)
            {
                return InputSources.Pop();
            }
            return null;
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

        public float GetAxisRaw(string axis)
        {
            return InputSources.Peek().GetAxisRaw(axis);
        }

        public Vector2 GetAxis2DRaw(string xAxis, string yAxis)
        {
            return InputSources.Peek().GetAxis2DRaw(xAxis, yAxis);
        }

        public Vector2 mousePosition => InputSources.Peek().mousePosition;

    }


}