using Assets.Code.Zones;
using UnityEngine;
using System.Linq;

namespace Assets.Code
{

    public class Spawn : MonoBehaviour
    {

        public ZonePersistence ObjectToSpawn;

        private void Start()
        {
            if (FindObjectsOfType<ZonePersistence>().FirstOrDefault(zp => zp.guid.Equals(ObjectToSpawn.guid)) == null)
            {
                Instantiate(ObjectToSpawn);
            }
        }

    }

}