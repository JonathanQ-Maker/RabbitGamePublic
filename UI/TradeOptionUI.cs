using UnityEngine;
using UnityEngine.EventSystems;

public class TradeOptionUI : MonoBehaviour, ISelectHandler, IItemContainer
{
    private TradeOption option;
    public TradeOption Option 
    {
        get { return option; }
        set 
        { 
            option = value;
            fromSlot.SetContainer(0, option);
        }
    }

    public bool Selected 
    {
        get { return selectBorder.activeSelf; }
        set { selectBorder.SetActive(value); }
    }

    [SerializeField]
    private ItemSlotUI fromSlot, toSlot;
    [SerializeField]
    private GameObject selectBorder;
    private ItemStack previewItem;

    public delegate void OnSelectEvent(TradeOptionUI option, bool selected);
    public OnSelectEvent onSelectHandler;

    public void UpdateRender(bool ownerView)
    {
        toSlot.Background = ownerView;
        toSlot.interactable = ownerView;
        if (ownerView)
        {
            toSlot.SetContainer(1, option);
        }
        else 
        {
            if (option.To == null)
            {
                previewItem = null;
            }
            else
            {
                previewItem = new ItemStack(option.To.Item);
            }
            toSlot.SetContainer(0, this);
        }
        fromSlot.UpdateRender();
        toSlot.UpdateRender();
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelectHandler?.Invoke(this, true);
    }

    private void OnDestroy()
    {
        onSelectHandler?.Invoke(this, false);
    }

    private void OnDisable()
    {
        onSelectHandler?.Invoke(this, false);
    }

    public ItemStack GetItem(int index)
    {
        return previewItem;
    }

    public void SetItem(int index, ItemStack itemStack)
    {
        previewItem = itemStack;
    }
}