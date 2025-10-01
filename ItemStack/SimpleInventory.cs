using UnityEngine;

[System.Serializable]
public class SimpleInventory : IItemContainer
{
    [SerializeField]
    private ItemStack[] items;

    public SimpleInventory(int size)
    { 
        items = new ItemStack[size];
    }

    public int Length => items.Length;

    public ItemStack GetItem(int index)
    {
        return items[index];
    }

    public void SetItem(int index, ItemStack itemStack)
    {
        items[index] = itemStack;
    }

    public int AddItem(ItemStack itemStack)
    {
        int countAdded = 0;
        int openSlot = -1;
        for (int i = 0; i < Length && itemStack.Count > 0; i++)
        {
            ItemStack itemAtSlot = items[i];
            if (itemAtSlot == null)
            {
                if (openSlot < 0) openSlot = i;
            }
            else 
            { 
                countAdded += itemAtSlot.CombineStack(itemStack, itemStack.Count);
            }
        }

        if (itemStack.Count != 0 && openSlot >= 0) {
            SetItem(openSlot, itemStack);
            countAdded += itemStack.Count;
        }
        return countAdded;
    }
}