using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code.References
{
    public class SceneLoadingZone : ScriptableObject 
    {

        public string Name;
        public List<SceneVariable> Scenes = new List<SceneVariable>();

    }
}
