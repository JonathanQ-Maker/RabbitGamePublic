using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeOptionUI : MonoBehaviour, ISelectHandler
{
    private TradeOption option;
    public TradeOption Option 
    {
        get { return option; }
        set 
        { 
            option = value;
            fromSlot.SetContainer(0, option);
            toSlot.SetContainer(1, option);
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

    public delegate void OnSelectEvent(TradeOptionUI option, bool selected);
    public OnSelectEvent onSelectHandler;

    public void UpdateRender(bool ownerView)
    {
        toSlot.Background = ownerView;
        toSlot.interactable = ownerView;
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
}