using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int slotIndex = -1;
    public int SlotIndex { get { return slotIndex; } }
    private IItemContainer container;
    public bool interactable;
    public bool Background 
    {
        get { return background.activeSelf; }
        set { background.SetActive(value); }
    }

    [SerializeField]
    private GameObject background;
    [SerializeField]
    private UIItemRenderer uiItemPrefab;
    [SerializeField]
    private Image overlay;
    [SerializeField]
    private Transform itemHolder;
    public Transform ItemHolder { get { return itemHolder; } }


    public void SetContainer(int slotIndex, IItemContainer container)
    {
        this.slotIndex = slotIndex;
        this.container = container;
    }

    public void UpdateRender()
    {
        // TODO: render the item
        foreach (Transform child in itemHolder)
        {
            Debug.Log("Slot UpdateRender destroy");
            Destroy(child.gameObject);
        }

        if (container == null) return;
        if (slotIndex < 0) return;

        ItemStack itemInSlot = GetItem();
        if (itemInSlot != null)
        {
            UIItemRenderer uiItem = Instantiate(uiItemPrefab, itemHolder);
            uiItem.ItemStack = itemInSlot;
            uiItem.UpdateRender();
            uiItem.parentSlot = this;
        }
    }

    public bool TryMountItem(UIItemRenderer uiItem)
    {
        if (!interactable) return false;
        if (GetItem() != null) return false;

        uiItem.parentSlot = this;
        SetItem(uiItem.ItemStack);
        return true;
    }

    public void OnDismountItem()
    {
        overlay.enabled = false;
        SetItem(null);
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable) return;
        overlay.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        overlay.enabled = false;
    }







    public ItemStack GetItem()
    { 
        return container.GetItem(slotIndex);
    }

    public void SetItem(ItemStack itemStack)
    { 
        container.SetItem(slotIndex, itemStack);
    }
}