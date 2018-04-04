using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{

    public class God : MonoBehaviour
    {

        private static God Instance = null;
        public GameState.GameState GameState;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }

    }


}