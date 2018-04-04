using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.References
{

    [CreateAssetMenu(fileName = "Loading Zone Reference.asset", menuName = "Status 92/Scene Loading Zone", order = 2)]
    public class SceneLoadingZone : ScriptableObject 
    {

        public string Name;
        public List<SceneVariable> Scenes = new List<SceneVariable>();

        public GameObject SceneContainer;

        public void AddScene(SceneVariable scene)
        {
            if (Scenes.Find(s => s.Value == scene.Value) == null)
            {
                Scenes.Add(scene);
            }
        }

        public void RemoveScene(SceneVariable scene)
        {
            Scenes = Scenes.Where(s => s == null || s.Value != scene.Value).ToList();
        }

    }
}
