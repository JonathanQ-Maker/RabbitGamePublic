using UnityEngine;

public class WorldItemRenderer : MonoBehaviour
{
    [SerializeField]
    private Item template;
    private ItemStack itemStack;
    public ItemStack ItemStack 
    {
        get { return itemStack; }
        set { itemStack = value; }
    }




    private void Start()
    {
        if (ItemStack == null) {
            // renderer exists but no item, create from template
            ItemStack = new ItemStack(template);
        }
    }
}