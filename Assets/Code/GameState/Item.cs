using System;
using UnityEngine;

namespace Assets.Code.GameState
{
    [CreateAssetMenu(fileName = "Item.asset", menuName = "Status 92/Inventory Item", order = 41)]
    public class Item : ScriptableObject, IComparable<Item>
    {

        public string DisplayName = "New Item";
        public string Description = "Description";

        public Sprite DisplayIcon = null;
        public ItemCategory Category = null;

        public int CompareTo(Item other)
        {
            return DisplayName.CompareTo(other.DisplayName);
        }
    }
}
