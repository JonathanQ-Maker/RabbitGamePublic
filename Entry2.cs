using UnityEngine;

public class Entry2 : MonoBehaviour 
{
    public TradeMenu tradeMenu;
    public Item coin, book, twig;
    public bool ownerView;

    private void Start()
    {
        ItemStack coinStack = new ItemStack(coin);
        coinStack.Count = 99;

        ItemStack books = new ItemStack(book);
        books.Count = 99;
        TradeModel model = new TradeModel();
        model.AddTrade(new ItemStack(coin), books);
        model.AddTrade(new ItemStack(coin), null);
        model.AddTrade(new ItemStack(coin), null);
        model.AddTrade(new ItemStack(coin), null);
        model.AddTrade(new ItemStack(coin), null);
        model.Offer = coinStack;
        model.To = new ItemStack(twig);
        tradeMenu.Model = model;
        tradeMenu.UpdateRender(ownerView);
    }
}