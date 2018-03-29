using UnityEngine;
using System.Collections.Generic;

using Assets.Code.References;

namespace Assets.Code
{

    [RequireComponent(typeof(BoxCollider))]
    public class ZoneLoadingTrigger : MonoBehaviour
    {

        public bool ForceTrigger = true;
        public List<SceneVariable> Scenes;

        private void Start()
        {
            if (ForceTrigger)
            {
                GetComponent<BoxCollider>().isTrigger = true;
            }
        }

    }

}