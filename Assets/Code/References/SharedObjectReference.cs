using UnityEngine;
using System;
using Newtonsoft.Json;

namespace Assets.Code.References
{

    [CreateAssetMenu(fileName = "Object reference.asset", menuName = "Status 92/Shared Object Reference", order = 26)]
    public class SharedObjectReference : ScriptableObject
    {

        [NonSerialized]
        public UnityEngine.Object Value;

    }
}
