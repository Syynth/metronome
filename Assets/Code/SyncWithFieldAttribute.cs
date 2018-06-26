using UnityEngine;

namespace Assets.Code
{

    public class SyncWithFieldAttribute : PropertyAttribute
    {

        public readonly string Field;

        public SyncWithFieldAttribute(string Field)
        {
            this.Field = Field;
        }

    }
}
