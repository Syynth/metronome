using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Assets.Code;
using UnityEngine.UI;

namespace Assets.Code.GameState
{
    public class GameStateFileManager : MonoBehaviour
    {

        public GameState EditorGameState = null;
        public Button LoadButton = null;

        [SerializeField]
        string[] saveFiles = new string[0];

        public void DetectSaveGames()
        {
            saveFiles = Directory.GetFileSystemEntries(Utils.SaveFileDirectory, "*.json");
        }

        private void Start()
        {
            DetectSaveGames();
        }

    }

}

