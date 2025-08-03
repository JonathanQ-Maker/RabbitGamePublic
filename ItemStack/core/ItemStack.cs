using UnityEngine;

[System.Serializable]
public class ItemStack
{
    [SerializeField]
    private ItemMaterial itemMaterial;
    public ItemMaterial ItemMaterial { get { return itemMaterial; } }

    [SerializeField]
    private string itemName = "Item";
    public string Name
    {
        get { return itemName; }
        set { itemName = value; }
    }

    [SerializeField]
    [Range(1, 99)]
    private int count = 1;
    public int Count
    {
        get { return count; }
        set { count = value; }
    }










    public ItemStack(string itemName, int count, int maxStackCount)
    { 
        this.itemName = itemName;
        this.count = count;
    }

    public ItemStack(ItemStack other)
    { 
        itemName = other.itemName;
        count = other.count;
    }

    public virtual bool CanCombine(ItemStack other)
    {
        if (!ReferenceEquals(ItemMaterial, other.ItemMaterial))
            return false;

        if (ItemMaterial.MaxStackCount != other.ItemMaterial.MaxStackCount)
            return false;

        if (Count + other.Count > ItemMaterial.MaxStackCount)
            return false;

        return !Name.Equals(other.Name);
    }

    public int TransferToStack(ItemStack from, int amountRequested)
    { 
        if (!CanCombine(from)) return 0;

        int amountTransferred = Mathf.Min(amountRequested, ItemMaterial.MaxStackCount - Count, from.Count);
        amountTransferred = Mathf.Max(amountTransferred, 0);
        Count += amountTransferred;
        from.Count -= amountTransferred;

        return amountTransferred;
    }

    public int SplitStack(out ItemStack newStack, int amountRequested)
    {
        int amountSplit = Mathf.Min(amountRequested, Count, ItemMaterial.MaxStackCount);
        amountSplit = Mathf.Max(amountSplit, 0);
        
        if (amountSplit == 0)
        { 
            newStack = null;
            return amountSplit;
        }

        newStack = new ItemStack(this);
        Count -= amountSplit;
        newStack.Count = amountSplit;
        return amountSplit;
    }
}