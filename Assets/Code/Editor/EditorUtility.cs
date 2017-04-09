using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Assets.Code.Editor
{
    class EditorUtility : MonoBehaviour
    {

        public static void CreateAsset<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
        }
        
    }

}