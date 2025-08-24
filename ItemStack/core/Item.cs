using UnityEngine;


[CreateAssetMenu(fileName = "ItemStack", menuName = "ScriptableObjects/ItemStack", order = 1)]
public class Item : ScriptableObject
{
    [SerializeField]
    private WorldItemRenderer worldItemPrefab;
    public WorldItemRenderer WorldItemRenderer { get { return worldItemPrefab; } }


    [SerializeField]
    [Range(1, 99)]
    private int maxStackCount;
    public int MaxStackCount { get { return maxStackCount; } }

    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; } }




    public virtual string GetName(ItemStack itemStack)
    {
        return itemStack.Data.Value<string>("name");
    }

    public virtual void SetName(ItemStack itemStack, string name)
    { 
        itemStack.Data["name"] = name;
    }



    public virtual int GetCount(ItemStack itemStack)
    {
        return itemStack.Data.Value<int>("count");
    }

    public virtual void SetCount(ItemStack itemStack, int count)
    {
        itemStack.Data["count"] = count;
    }

    public virtual void Initalize(ItemStack itemStack)
    {
        SetName(itemStack, name);
        SetCount(itemStack, 1);
    }


    public virtual bool CanCombine(ItemStack itemStack, ItemStack other)
    {
        if (itemStack == null || other == null) return false;

        if (ReferenceEquals(itemStack, other)) return false;

        if (!ReferenceEquals(itemStack.Item, other.Item)) return false;

        if (GetCount(itemStack) + GetCount(other) > MaxStackCount)
            return false;

        if (!GetName(itemStack).Equals(GetName(other))) return false;

        return true;
    }

    public virtual int TransferToStack(ItemStack itemStack, ItemStack from, int amountRequested)
    {
        if (!CanCombine(itemStack, from)) return 0;

        int amountTransferred = Mathf.Min(amountRequested, MaxStackCount - GetCount(itemStack), GetCount(from));
        amountTransferred = Mathf.Max(amountTransferred, 0);
        SetCount(itemStack, GetCount(itemStack) + amountTransferred);
        SetCount(from, GetCount(from) - amountTransferred);

        return amountTransferred;
    }

    public virtual int SplitStack(ItemStack itemStack, out ItemStack newStack, int amountRequested)
    {
        int amountSplit = Mathf.Min(amountRequested, GetCount(itemStack) - 1, MaxStackCount);
        amountSplit = Mathf.Max(amountSplit, 0);

        if (amountSplit == 0)
        {
            newStack = null;
            return amountSplit;
        }

        newStack = new ItemStack(itemStack);
        SetCount(itemStack, GetCount(itemStack) - amountSplit);
        SetCount(newStack, amountSplit);
        return amountSplit;
    }
}