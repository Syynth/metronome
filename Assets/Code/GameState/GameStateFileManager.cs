using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Assets.Code;

namespace Assets.Code.GameState
{
    public class GameStateFileManager : MonoBehaviour
    {

        public GameState EditorGameState = null;

        [SerializeField]
        string[] saveFiles;

        public void DetectSaveGames()
        {
            saveFiles = Directory.GetFileSystemEntries(Utils.SaveFileDirectory, "*.json");
        }

        private void Start()
        {
            DetectSaveGames();
        }

        public void LoadGameState(string fileName)
        {
            var jsonText = File.ReadAllText(Utils.SaveFileDirectory + fileName);
            JsonUtility.FromJsonOverwrite(jsonText, EditorGameState);
        }

        public void SaveGameState(GameState gameState)
        {
            var jsonText = JsonUtility.ToJson(gameState);
            var fileName = gameState.LastSave + ".json";
            File.WriteAllText(Utils.SaveFileDirectory + fileName, jsonText);
        }

    }

}

