using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Code.References;
using Assets.Code.Player;

namespace Assets.Code.Zones
{

    [RequireComponent(typeof(BoxCollider))]
    public class ZoneLoadingTrigger : MonoBehaviour
    {

        public bool ForceTrigger = true;
        public SceneVariable Scene;

        public SharedObjectReference ZoneManager;

        private void Start()
        {
            if (ForceTrigger)
            {
                GetComponent<BoxCollider>().isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.GetComponent<PlayerActor>() != null)
            {
                var zm = ZoneManager.Value as ZoneLoadingManager;
                zm.EnterScene(Scene, loadSceneMode: LoadSceneMode.Additive);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.GetComponent<PlayerActor>() != null)
            {
                var zm = ZoneManager.Value as ZoneLoadingManager;
                zm.ExitScene(Scene);
            }
        }

    }

}