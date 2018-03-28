using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Assets.Code.References;

namespace Assets.Code
{
    public class SceneEntrance : MonoBehaviour
    {

        public SceneVariable Target;
        public GameState.GameState GameState;

        public bool TransitionImmediately = true;

        public void Enter()
        {
            GameState.CurrentScene = Target;
            GameState.Save();
            Target.GoTo();
        }

    }
}
