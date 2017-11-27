using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

using Assets.Code;

namespace Assets.Code.Player
{

    public class RewiredInputSource : MonoBehaviour, IInputSource
    {

        Rewired.Player Input;

        void Start()
        {
            Input = ReInput.players.GetPlayer(0);
        }

        public float GetAxis(string axis)
        {
            return Input.GetAxis(axis);
        }

        public Vector2 GetAxis2D(string xAxis, string yAxis)
        {
            return Input.GetAxis2D(xAxis, yAxis);
        }

        public float GetAxisRaw(string axis)
        {
            return Input.GetAxisRaw(axis);
        }

        public Vector2 GetAxis2DRaw(string xAxis, string yAxis)
        {
            return Input.GetAxis2DRaw(xAxis, yAxis);
        }

        public bool GetButton(string button)
        {
            return Input.GetButton(button);
        }

        public bool GetButtonDown(string button)
        {
            return Input.GetButtonDown(button);
        }

        public Vector2 mousePosition => ReInput.controllers.Mouse.screenPosition;
        
    }

}