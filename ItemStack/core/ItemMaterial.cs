using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemMaterial : ScriptableObject
{
    [SerializeField]
    private WorldItemRenderer worldItemPrefab;
    public WorldItemRenderer WorldItemRenderer { get { return worldItemPrefab; } }


    [SerializeField]
    [Range(1, 99)]
    private int maxStackCount;
    public int MaxStackCount
    {
        get { return maxStackCount; }
    }
}