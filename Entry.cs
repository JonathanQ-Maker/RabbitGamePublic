using UnityEngine;

public class Entry : MonoBehaviour
{
    public SimpleInventoryRenderer inventoryRenderer;
    public Item item;
    public bool[] bools = new bool[6];
    private SimpleInventory inventory;
    void Start()
    {
        Application.targetFrameRate = 100;
        inventory = new SimpleInventory(6);
        inventory.SetItem(1, new ItemStack(item));
        inventory.SetItem(0, new ItemStack(item));
        inventoryRenderer.Inventory = inventory;
        inventoryRenderer.UpdateRender();
    }

    private void Update()
    {
        for (int i = 0; i < inventory.Length; ++i)
        {
            bools[i] = inventory.GetItem(i) != null;
        }
    }
}
