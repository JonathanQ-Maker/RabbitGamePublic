public interface IItemContainer
{
    int Length { get; }
    ItemStack GetItem(int index);

    void SetItem(int index, ItemStack itemStack);
}