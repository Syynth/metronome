using UnityEngine;

namespace Assets.Code.GameState
{

    [CreateAssetMenu(fileName = "Part.asset", menuName = "Status 92/Act Part", order = 61)]
    public class Part : ScriptableObject
    {

        public string DisplayName;

        public Part NextPart;
        public Part PreviousPart;

        public Act Act;

    }
}
