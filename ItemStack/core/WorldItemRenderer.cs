using UnityEngine;

public class WorldItemRenderer : MonoBehaviour
{
    [SerializeField]
    private ItemStack item;
    public ItemStack Item 
    {
        get { return item; }
        set { item = value; }
    }
}