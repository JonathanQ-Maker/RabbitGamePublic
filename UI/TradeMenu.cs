using UnityEngine;
using UnityEngine.UI;

public class TradeMenu : MonoBehaviour
{
    private TradeModel model;
    public TradeModel Model
    {
        get { return model; }
        set 
        { 
            model = value;
            collectionSlot.SetContainer(0, model);
            fromSlot.SetContainer(1, model);
            toSlot.SetContainer(2, model);
        }
    }

    [SerializeField]
    private TradeOptionUI optionPrefab;
    [SerializeField]
    private Transform optionsHolder;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private GameObject collection;
    [SerializeField]
    private GameObject exchange;
    [SerializeField]
    private ItemSlotUI collectionSlot;
    [SerializeField]
    private ItemSlotUI fromSlot, toSlot;

    private TradeOptionUI selectedOption;
    public void OnSliderValueChange() 
    {
        //Debug.Log($"TradeMenu slider changed. Value = {slider.value}");
    }

    public void OnOptionSelectEvent(TradeOptionUI option, bool selected)
    {
        if (selectedOption != null) selectedOption.Selected = false;
        selectedOption = selected ? option : null;
        if (selectedOption != null) selectedOption.Selected = true;
    }

    public void UpdateRender(bool ownerView)
    {
        selectedOption = null;
        foreach (Transform child in optionsHolder) 
        { 
            Destroy(child.gameObject);
        }

        for (int i = 0; i < model.NumTrades; i++)
        {
            TradeOptionUI option = Instantiate(optionPrefab, optionsHolder);
            option.Option = model.GetTrade(i);
            option.UpdateRender(ownerView);
            option.onSelectHandler = OnOptionSelectEvent;
        }

        slider.gameObject.SetActive(ownerView);
        collection.SetActive(ownerView);
        exchange.SetActive(!ownerView);
        collectionSlot.UpdateRender();
        fromSlot.UpdateRender();
        toSlot.UpdateRender();
    }
}