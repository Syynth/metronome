using System;

namespace Assets.Code.GameState
{

    [Serializable]
    public class ItemReference : IComparable<ItemReference>
    {

        public int quantity;

        public Item Item;

        public int CompareTo(ItemReference other)
        {
            if (Item == null)
            {
                return 1;
            }
            if (other?.Item == null)
            {
                return -1;
            }
            return Item.CompareTo(other.Item);
        }
    }
}
