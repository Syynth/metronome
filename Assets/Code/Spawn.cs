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
            if (FindObjectsOfType<ZonePersistence>().FirstOrDefault(zp => zp.guid.Equals(ObjectToSpawn.guid)) == null)
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