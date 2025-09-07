using UnityEngine;

public class Entry : MonoBehaviour
{
    public SimpleInventoryRenderer inventoryRenderer;
    public Item item;
    private SimpleInventory inventory;
    void Start()
    {
        Application.targetFrameRate = 100;
        inventory = new SimpleInventory(30);
        inventory.SetItem(1, new ItemStack(item));
        ItemStack itemStack = new ItemStack(item);
        itemStack.Count = 2;
        inventory.SetItem(0, itemStack);
        inventoryRenderer.Inventory = inventory;
        inventoryRenderer.UpdateRender();
    }

    private void Update()
    {

    }
}
