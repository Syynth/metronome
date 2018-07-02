using System;
using System.Linq;

using UnityEngine;

using Assets.Code.References;
using Assets.Code.Zones;

namespace Assets.Code
{

    public class Spawn : MonoBehaviour
    {

        public ZonePersistence ObjectToSpawn;

        public string guid = Guid.NewGuid().ToString();

        private void Start()
        {
            if (ObjectToSpawn == null)
            {
                return;
            }
            var zps = FindObjectsOfType<ZonePersistence>();
            zps.Select(zp => zp.guid).ToList().ForEach(s => Debug.Log(s));
            Debug.Log(ObjectToSpawn.guid);
            if (zps.FirstOrDefault(
                zp => zp.guid.Equals(ObjectToSpawn.guid)
            ) == null)
            {
                SpawnObject();
            }
        }

        public void SpawnObject()
        {
            var zp = Instantiate(ObjectToSpawn, transform.position, transform.rotation);
            zp.SceneReady(zp.Zone);
        }

    }

}