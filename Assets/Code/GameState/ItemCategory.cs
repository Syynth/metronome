using System;
using UnityEngine;

namespace Assets.Code.GameState
{

    [CreateAssetMenu(fileName = "Item Category.asset", menuName = "Status 92/Item Category", order = 42)]
    public class ItemCategory : ScriptableObject, IComparable<ItemCategory>
    {

        public string DisplayName = "Item Category";
        public string Description = "Description??";

        public Sprite DisplayIcon = null;

        public bool IsEvidence;
        public int SortingIndex = 0;

        public int CompareTo(ItemCategory other)
        {
            return SortingIndex.CompareTo(other.SortingIndex);
        }

    }
}
