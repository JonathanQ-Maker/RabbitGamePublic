using System.Collections.Generic;

public class TradeModel : IItemContainer
{
    private ItemStack collecion;
    private ItemStack[] exchangeSlots;
    private List<TradeOption> options;

    public int NumTrades
    {
        get { return options.Count; }
    }

    public ItemStack Offer {
        get { return exchangeSlots[0]; }
        set { exchangeSlots[0] = value; }
    }

    public ItemStack To
    {
        get { return exchangeSlots[1]; }
        set { exchangeSlots[1] = value; }
    }

    public TradeModel()
    {
        exchangeSlots = new ItemStack[2];
        options = new List<TradeOption>();
    }

    public ItemStack GetItem(int index)
    {
        if (0 == index) return collecion;
        return exchangeSlots[index - 1];
    }

    public void SetItem(int index, ItemStack itemStack)
    {
        if (0 == index) 
        {
            collecion = itemStack;
            return;
        }
        exchangeSlots[index - 1] = itemStack;
    }

    public void AddTrade(ItemStack from, ItemStack to)
    {
        options.Add(new TradeOption(from, to));
    }

    public TradeOption GetTrade(int index) 
    {
        return options[index];
    }

    public void RemoveTrade(int index)
    {
        options.RemoveAt(index);
    }

    public bool CanTrade(int tradeIndex, ItemStack offer)
    {
        if (To != null && (!To.IsSimilar(options[tradeIndex].To) || To.Count == To.Item.MaxStackCount)) return false;
        if (options[tradeIndex].From == null || options[tradeIndex].To == null) return false;
        if (options[tradeIndex].From.IsSimilar(offer) && options[tradeIndex].To.Count >= 1)
        {
            if (offer.Count >= options[tradeIndex].From.Count)
            {
                if (collecion == null) return true;

                return collecion.IsSimilar(offer) && (collecion.Count + options[tradeIndex].From.Count) <= collecion.MaxStackCount;
            }
        }
        return false;
    }

    public void Trade(int tradeIndex)
    {
        if (!CanTrade(tradeIndex, Offer)) return;

        Offer.SplitStack(out ItemStack income, options[tradeIndex].From.Count);
        if (Offer.Count == 0) Offer = null;


        // put the offer into collection
        if (collecion == null)
        {
            collecion = income;
        }
        else
        {
            collecion.CombineStack(income, income.Count);
        }

        // take one item out of to trade stack
        options[tradeIndex].To.SplitStack(out ItemStack cost, 1);
        if (options[tradeIndex].To.Count == 0) options[tradeIndex].To = null;

        if (To == null)
        {
            To = cost;
        }
        else 
        { 
            To.CombineStack(cost, cost.Count);
        }
    }
}

public class TradeOption : IItemContainer
{
    public ItemStack From 
    {
        set { items[0] = value; }
        get { return items[0]; }
    }
    public ItemStack To 
    {
        set { items[1] = value; }
        get { return items[1]; }
    }

    private ItemStack[] items;

    public TradeOption(ItemStack from, ItemStack to) 
    {
        items = new ItemStack[] { from, to };
    }

    public ItemStack GetItem(int index)
    {
        return items[index];
    }

    public void SetItem(int index, ItemStack itemStack)
    {
        items[index] = itemStack;
    }
}