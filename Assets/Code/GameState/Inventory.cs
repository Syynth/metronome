using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Code.GameState
{
    [CreateAssetMenu(fileName = "Inventory.asset", menuName = "Status 92/Inventory", order = 40)]
    public class Inventory : ScriptableObject
    {

        public enum InventorySortModes
        {
            Alphabetical = 0,
            Category = 1,
        }

        public List<ItemReference> Items = new List<ItemReference>();

        public Inventory Clone() => CreateInstance<Inventory>();

        public void AddItemToInventory(Item item, int quantity)
        {
            AddItemToInventory(new ItemReference() { Item = item, quantity = quantity });
        }

        public void AddItemToInventory(ItemReference itemReference)
        {
            var existingRef = Items.FirstOrDefault(itemRef => itemRef.Item == itemReference.Item);
            if (existingRef != null)
            {
                existingRef.quantity += itemReference.quantity;
            }
            else
            {
                Items.Add(itemReference);
            }
        }

        public void RemoveItemFromInventory(Item item, int quantity)
        {
            var existingRef = Items.FirstOrDefault(itemRef => itemRef.Item == item);
            if (existingRef != null)
            {
                existingRef.quantity -= quantity;
                if (existingRef.quantity < 0 || quantity == 0)
                {
                    Items.Remove(existingRef);
                }
            }
        }

        public void SortItems(InventorySortModes sortingMode)
        {
            switch (sortingMode)
            {
                case InventorySortModes.Alphabetical:
                    SortAlphabetical();
                    break;
                case InventorySortModes.Category:
                    SortCategory();
                    break;
            }
        }

        private void SortAlphabetical()
        {
            Items.Sort();
        }

        private void SortCategory()
        {
            Items = Items.GroupBy(item => item.Item?.Category).OrderBy(obj => obj.Key).SelectMany(g => g).ToList();
        }

    }
}
