using UnityEngine;

public class InventoryItemRenderer
{
    [SerializeField]
    private ItemStack item;
    public ItemStack Item
    {
        get { return item; }
        set { item = value; }
    }
}