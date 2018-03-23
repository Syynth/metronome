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

        public string LastSave = DateTime.Now.ToString(Utils.DateFormat);

        public void Save()
        {
            Directory.CreateDirectory(Utils.SaveFileDirectory);
            var jsonText = JsonUtility.ToJson(this);
            var previousFileName = Utils.SaveFileDirectory + LastSave + ".json";
            File.WriteAllText(previousFileName, jsonText);
            LastSave = DateTime.Now.ToString(Utils.DateFormat);
            var nextFileName = Utils.SaveFileDirectory + LastSave + ".json";
            File.Move(previousFileName, nextFileName);
        }

    }
}
