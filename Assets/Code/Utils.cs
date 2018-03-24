using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;

namespace Assets.Code
{

    public static class JsonSerializer
    {

        public static string SerializeRecursive(object target)
        {
            return SerializeRecursiveWithoutCircular(target, new HashSet<string>());
        }

        private static string SerializeRecursiveWithoutCircular(object target, HashSet<string> set)
        {
            if (target == null) return "null";
            string baseJson = JsonUtility.ToJson(target);

            var data = JObject.Parse(baseJson);
            data.Properties()
                .Where(prop => {
                    Debug.Log("Testing prop " + prop.Name + ", Count=" + prop.Count + ", Value.Type=" + prop.Value.Type);
                    return prop.Value.Type == JTokenType.Object &&
                        prop.Count == 1 &&
                        prop.Value["instanceID"] != null;
                })
                .ToList()
                .ForEach(prop =>
                {
                    var instanceID = (string)prop.Value["instanceID"];
                    Debug.Log("Recursing into prop " + prop.Name + ", instanceID=" + instanceID);
                    if (set.Contains(instanceID) || instanceID == "0") return;
                    (prop.Value as JObject)
                        .Property("instanceID")
                        .AddAfterSelf(
                            new JProperty(
                                "instanceData",
                                SerializeRecursiveWithoutCircular(target.GetType().GetField(prop.Name).GetValue(target), set)
                            )  
                        );
                    Debug.Log(prop);
                    set.Add(instanceID);
                });
            return data.ToString();
        }

    }

    public static class Direction
    {
        public const int Left = -1;
        public const int Right = 1;
    }

    public static class Utils
    {

        public static readonly string SaveFileDirectory = Application.dataPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar;
        public const string DateFormat = "yyyy-MM-dd HH.mm.ss";

        public static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }

        public static Vector3 Clockwise(Vector3 input)
        {
            return new Vector3(-input.y, input.x);
        }

        public static Vector3 CounterClockwise(Vector3 input)
        {
            return Clockwise(input) * -1;
        }

        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

    }

}