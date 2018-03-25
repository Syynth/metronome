using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Assets.Code.References
{

    [CreateAssetMenu(fileName = "Scene Reference.asset", menuName = "Status 92/Scene Variable", order = 1)]
    public class SceneVariable : ScriptableObject
    {

        public string Value = null;

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