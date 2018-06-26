using System.Collections.Generic;

using UnityEngine;

namespace Assets.Code.GameState
{

    [CreateAssetMenu(fileName = "Act.asset", menuName = "Status 92/Act", order = 60)]
    public class Act : ScriptableObject
    {

        public string DisplayName;

        [SyncWithField(Field: "PreviousAct")]
        public Act NextAct;

        [SyncWithField(Field: "NextAct")]
        public Act PreviousAct;

        public List<Part> Parts;

    }
}
