using UnityEngine;
using System;
using System.Linq;
using Assets.Code.References;

namespace Assets.Code.Zones
{

    public class ZonePersistence : MonoBehaviour
    {

        public SharedObjectReference ZoneLoadingManagerReference;
        public SceneLoadingZone Zone;

        public string guid;

        public ZoneLoadingManager ZoneLoadingManager
        {
            get
            {
                return ZoneLoadingManagerReference.Value as ZoneLoadingManager;
            }
        }

        public void OnEnable()
        {
            if (guid == null)
            {
                guid = Guid.NewGuid().ToString();
            }
        }

        private void Awake()
        {
            var zps = FindObjectsOfType<ZonePersistence>().Where(zp => zp.guid.Equals(guid)).ToList();
            if (zps.Count() > 1 &&
                transform.parent != Zone.SceneContainer.transform)
            {
                gameObject.SetActive(false);
                DestroyImmediate(gameObject);
            }
        }

    }

}
