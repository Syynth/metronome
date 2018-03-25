using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Code;
using System.IO;

namespace Assets.Code.GameState
{
    [CreateAssetMenu(fileName = "gamestate.asset", menuName = "Status 92/Game State", order = 0)]
    public class GameState : ScriptableObject
    {

        public string SaveName = "New Save";
        public string LastSave = DateTime.Now.ToString(Utils.DateFormat);

        public GameState TestState = null;

        public void Save()
        {
            Directory.CreateDirectory(Utils.SaveFileDirectory);
            var jsonText = JsonSerializer.SerializeRecursive(this);
            var fileName = Utils.SaveFileDirectory + SaveName + ".json";
            File.WriteAllText(fileName, jsonText);
            LastSave = DateTime.Now.ToString(Utils.DateFormat);
        }

        public void Load(string filePath)
        {
            string jsonText = File.ReadAllText(filePath);
            JsonSerializer.DeserializeRecursive<GameState>(jsonText, this);
        }

    }
}
