using Newtonsoft.Json.Linq;
using UnityEngine;

public class ItemStack
{
    [SerializeField]
    private Item item;
    public Item Item { get { return item; } }

    private JObject data;
    public JObject Data { get { return data; } }

    public int MaxStackCount { get { return Item.MaxStackCount; } }

    public int Count 
    {
        get { return Item.GetCount(this); }
        set { Item.SetCount(this, value); }
    }

    public string Name
    {
        get { return Item.GetName(this); }
        set { Item.SetName(this, value); }
    }

    public Sprite Sprite { get { return Item.Sprite; } }












    public ItemStack(Item item)
    {
        this.item = item;
        data = new JObject();
        item.Initalize(this);
    }

    public ItemStack(ItemStack original)
    {
        item = original.Item;
        data = (JObject)original.Data.DeepClone();
    }



    public bool CanCombine(ItemStack other)
    {
        return Item.CanCombine(this, other);
    }

    public int CombineStack(ItemStack from, int amountRequested)
    {
        return Item.CombineStack(this, from, amountRequested);
    }

    public int SplitStack(out ItemStack newStack, int amountRequested)
    {
        return Item.SplitStack(this, out newStack,amountRequested);
    }

    public override string ToString()
    {
        return data.ToString();
    }
}