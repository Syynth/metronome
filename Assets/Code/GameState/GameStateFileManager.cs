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
            if (LoadButton != null)
            {
                LoadButton.gameObject.SetActive(saveFiles.Length > 0);
            }
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

