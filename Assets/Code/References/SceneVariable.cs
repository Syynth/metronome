using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.References
{

    [CreateAssetMenu(fileName = "Scene Reference.asset", menuName = "Status 92/Scene Variable", order = 1)]
    public class SceneVariable : ScriptableObject
    {

        public string Value = null;
        public SceneLoadingZone LoadingZone = null;
        public List<SceneVariable> ConnectedScenes = new List<SceneVariable>();

        public void SetLoadingZone(SceneLoadingZone zone)
        {
            if (LoadingZone != null & LoadingZone != zone)
            {
                LoadingZone.RemoveScene(this);
            }
            LoadingZone = zone;
        }

        public void GoTo()
        {
            SceneManager.LoadSceneAsync(Value, LoadSceneMode.Single);
        }

        public void LoadAsync()
        {
            SceneManager.LoadSceneAsync(Value, LoadSceneMode.Additive);
        }

    }

}