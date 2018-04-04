using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code.References
{

    [CreateAssetMenu(fileName = "Object reference.asset", menuName = "Status 92/Shared Object Reference", order = 4)]
    public class SharedObjectReference : ScriptableObject
    {

        public Object Value;

    }
}
