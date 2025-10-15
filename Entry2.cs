using UnityEngine;

public class Entry2 : MonoBehaviour 
{
    public TradeMenu tradeMenu;
    public Item coin, book, twig;
    public bool ownerView;

    private void Start()
    {
        TradeModel model = new TradeModel();
        model.AddTrade(new ItemStack(coin), new ItemStack(book));
        model.AddTrade(new ItemStack(coin), new ItemStack(twig));
        tradeMenu.Model = model;
        tradeMenu.UpdateRender(ownerView);
    }
}