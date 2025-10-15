public interface IItemContainer
{
    ItemStack GetItem(int index);

    void SetItem(int index, ItemStack itemStack);
}