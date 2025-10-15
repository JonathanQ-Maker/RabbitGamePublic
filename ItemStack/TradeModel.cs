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

    public ItemStack Trade(int tradeIndex, ItemStack offer) 
    { 
        if (!CanTrade(tradeIndex, offer)) return null;

        offer.SplitStack(out ItemStack income, options[tradeIndex].From.Count);
        if (collecion == null)
        {
            collecion = income;
        }
        else
        {
            collecion.CombineStack(income, income.Count);
        }

        options[tradeIndex].To.SplitStack(out ItemStack cost, 1);
        if (options[tradeIndex].To.Count == 0) 
        {
            RemoveTrade(tradeIndex);
        }
        return cost;
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
        items = new ItemStack[2] { from, to };
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