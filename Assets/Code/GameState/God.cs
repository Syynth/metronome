using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Assets.Code.Player;

namespace Assets.Code
{

    public class God : MonoBehaviour
    {

        private static God Instance = null;
        public GameState.GameState GameState;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this);
            }

            if (FindObjectOfType<PlayerActor>() == null && GameState.lastSpawnGuid != null)
            {
                FindObjectsOfType<Spawn>()
                    .ToList()
                    .Find(spawn => spawn.guid == GameState.lastSpawnGuid)
                    .SpawnObject();
            }
        }

    }


}