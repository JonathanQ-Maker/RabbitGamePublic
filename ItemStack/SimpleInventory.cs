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
}