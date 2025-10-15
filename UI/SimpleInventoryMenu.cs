using UnityEngine;

public class SimpleInventoryMenu : MonoBehaviour
{
    private SimpleInventory inventory;
    public SimpleInventory Inventory 
    {
        get { return inventory; }
        set { inventory = value; }
    }

    [SerializeField]
    private ItemSlotUI slotUIPrefab;


    public void UpdateRender()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (Inventory == null) return;
        for (int i = 0; i < Inventory.Length; ++i)
        {
            ItemSlotUI slotUI = Instantiate(slotUIPrefab, transform);
            slotUI.SetContainer(i, Inventory);
            slotUI.UpdateRender();
        }
    }
}