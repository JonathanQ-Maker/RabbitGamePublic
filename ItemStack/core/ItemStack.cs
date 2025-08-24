using Newtonsoft.Json.Linq;
using UnityEngine;

public class ItemStack
{
    [SerializeField]
    private Item item;
    public Item Item { get { return item; } }

    private JObject data;
    public JObject Data { get { return data; } }










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
}