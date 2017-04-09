using UnityEngine;
using UnityEditor;

namespace Assets.Code.Editor
{
    class EditorIntegrations : MonoBehaviour
    {

        [MenuItem("Assets/Create/Sprite Animation")]
        public static void BuildSpriteAnimation()
        {
            EditorUtility.CreateAsset<SpriteAnimation>();
        }
    }

}