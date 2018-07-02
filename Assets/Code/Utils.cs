using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
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
                    if (set.Contains(instanceID) || instanceID == "0") return;
                    var newSet = new HashSet<string>(set)
                    {
                        instanceID
                    };
                    (prop.Value as JObject)
                        .Property("instanceID")
                        .AddAfterSelf(
                            new JProperty(
                                "instanceData",
                                SerializeRecursiveWithoutCircular(target.GetType().GetField(prop.Name).GetValue(target), newSet)
                            )  
                        );
                });
            return data.ToString();
        }

        public static T DeserializeRecursiveOverwrite<T>(string json, T target)
        {
            var data = JObject.Parse(json);
            JsonUtility.FromJsonOverwrite(json, target);
            data.Properties()
                .Where(prop =>
                {
                    Debug.Log("Testing prop " + prop.Name + ", Value.Count=" + prop.Value.Count() + ", Value.Type=" + prop.Value.Type); // + ", instanceID=" + prop.Value["instanceID"] + ", instanceData=" + prop.Value["instanceData"]);
                    return prop.Value.Type == JTokenType.Object &&
                        prop.Value.Count() == 2 &&
                        prop.Value["instanceID"] != null &&
                        prop.Value["instanceData"] != null;
                })
                .ToList()
                .ForEach(prop =>
                {
                    var instanceID = (string)prop.Value["instanceID"];
                    var instanceData = (string)prop.Value["instanceData"];
                    Debug.Log("Recursing into prop " + prop.Name + ", instanceID=" + instanceID);
                    if (instanceID == "0") return;
                    var child = target.GetType().GetField(prop.Name).GetValue(target);
                    DeserializeRecursiveOverwrite(instanceData, child);
                });

            return target;
        }

        public static T DeserializeRecursive<T>(string json) where T : ScriptableObject
        {
            var target = ScriptableObject.CreateInstance<T>();
            return DeserializeRecursiveOverwrite(json, target);
        }

    }

    public static class Direction
    {
        public const int Left = -1;
        public const int Right = 1;
    }

    public static class Utils
    {

        public static readonly string SaveFileDirectory = Application.dataPath + "/saves/";
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

        public static List<T> FindObjectsOfTypeAll<T>()
        {
            List<T> results = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    var allGameObjects = s.GetRootGameObjects();
                    for (int j = 0; j < allGameObjects.Length; j++)
                    {
                        var go = allGameObjects[j];
                        results.AddRange(go.GetComponentsInChildren<T>(true));
                    }
                }
            }
            return results;
        }

        public static List<Type> GetSubTypesOf<T>()
        {
            return typeof(T)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(T)))
                .ToList();
        }

        public static List<object> InstantiateForTypes(List<Type> types)
        {
            return types.Select(t => Activator.CreateInstance(t)).ToList();
        }

    }

}