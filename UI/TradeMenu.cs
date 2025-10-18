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
    private bool ownerView;
    public void OnSliderValueChange() 
    {
        if (selectedOption == null) return;
        selectedOption.Option.From.Count = (int)slider.value;
        selectedOption.UpdateRender(ownerView);
    }

    public void OnOptionSelectEvent(TradeOptionUI option, bool selected)
    {
        if (selectedOption != null) selectedOption.Selected = false;
        selectedOption = selected ? option : null;
        if (selectedOption != null) selectedOption.Selected = true;
        slider.gameObject.SetActive(ownerView && selectedOption != null);

        if (selectedOption == null) return;
        slider.minValue = 1;
        slider.maxValue = 99;

        if (selectedOption.Option.From != null)
        {
            slider.value = selectedOption.Option.From.Count;
        }
    }

    public void OnClickTrade()
    {
        if (selectedOption == null) return;
        Model.Trade(selectedOption.transform.GetSiblingIndex());
        fromSlot.UpdateRender();
        toSlot.UpdateRender();
        selectedOption.UpdateRender(ownerView);
    }

    public void UpdateRender(bool ownerView)
    {
        this.ownerView = ownerView;
        selectedOption = null;
        foreach (Transform child in optionsHolder) 
        { 
            Destroy(child.gameObject);
        }

        slider.gameObject.SetActive(ownerView && selectedOption != null);
        collection.SetActive(ownerView);
        exchange.SetActive(!ownerView);
        collectionSlot.UpdateRender();
        fromSlot.UpdateRender();
        toSlot.UpdateRender();

        if (model == null) return;
        for (int i = 0; i < model.NumTrades; i++)
        {
            TradeOptionUI option = Instantiate(optionPrefab, optionsHolder);
            option.Option = model.GetTrade(i);
            option.UpdateRender(ownerView);
            option.onSelectHandler = OnOptionSelectEvent;

            if (i == 0) OnOptionSelectEvent(option, true);
        }
    }
}