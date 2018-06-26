using UnityEngine;
using System;
using System.Linq;
using Assets.Code.References;

namespace Assets.Code.Zones
{

    public class ZonePersistence : MonoBehaviour
    {

        public SharedObjectReference ZoneLoadingManagerReference = null;

        [SerializeField]
        private SceneLoadingZone m_Zone = null;
        public SceneLoadingZone Zone
        {
            get
            {
                return m_Zone ?? ZoneLoadingManager?.CurrentZone;
            }
            set
            {
                m_Zone = value;
            }
        }

        public string guid = Guid.NewGuid().ToString();

        public ZoneLoadingManager ZoneLoadingManager
        {
            get
            {
                return ZoneLoadingManagerReference?.Value as ZoneLoadingManager;
            }
        }

        public void SceneReady(SceneLoadingZone zone)
        {
            Zone = zone;
            Awake();
        }

        private void Start()
        {
            Awake();
        }

        private void Awake()
        {
            if (Zone == null) return;
            var zps = FindObjectsOfType<ZonePersistence>().Where(zp => zp.guid.Equals(guid)).ToList();
            if (Zone.SceneContainer == null)
            {
                var go = new GameObject();
                DontDestroyOnLoad(go);
                Zone.SceneContainer = go;
            }
            if (zps.Count() > 1)
            {
                if (transform.parent != Zone.SceneContainer.transform)
                {
                    gameObject.SetActive(false);
                    DestroyImmediate(gameObject);
                }
            }
            else
            {
                if (transform.parent == null)
                {
                    transform.parent = Zone.SceneContainer.transform;
                }
            }
        }

    }

}
