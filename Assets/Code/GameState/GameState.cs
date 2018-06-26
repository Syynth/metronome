using System;
using System.IO;

using UnityEngine;

using Assets.Code.References;
using Assets.Code.Zones;

namespace Assets.Code.GameState
{
    [CreateAssetMenu(fileName = "gamestate.asset", menuName = "Status 92/Game State", order = 0)]
    public class GameState : ScriptableObject
    {

        public string SaveName = "New Save";
        public string LastSave = DateTime.Now.ToString(Utils.DateFormat);

        public Inventory Inventory;

        public SceneVariable DefaultIntroScene;

        public SceneVariable CurrentScene;
        public SharedObjectReference ZoneLoadingManagerReference;

        public string lastSpawnGuid = null;

        public ZoneLoadingManager ZoneLoadingManager
        {
            get
            {
                return ZoneLoadingManagerReference.Value as ZoneLoadingManager;
            }
        }


        public void Reinitialize(string SaveName = "New Save")
        {
            this.SaveName = SaveName;
            CurrentScene = DefaultIntroScene;
        }

        public void StartGame()
        {
            Save();
            ZoneLoadingManager.EnterZone(CurrentScene.LoadingZone, CurrentScene, true);
        }

        public void Save()
        {
            Directory.CreateDirectory(Utils.SaveFileDirectory);
            var jsonText = JsonSerializer.SerializeRecursive(this);
            var fileName = Utils.SaveFileDirectory + SaveName + ".json";
            LastSave = DateTime.Now.ToString(Utils.DateFormat);
            File.WriteAllText(fileName, jsonText);
        }

        public void Load(string filePath)
        {
            string jsonText = File.ReadAllText(filePath);
            JsonSerializer.DeserializeRecursiveOverwrite(jsonText, this);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

    }
}
