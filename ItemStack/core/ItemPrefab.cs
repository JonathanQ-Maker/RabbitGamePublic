using UnityEngine;

[CreateAssetMenu(fileName = "ItemPrefab", menuName = "ScriptableObjects/ItemPrefab", order = 1)]
public class ItemPrefab : ScriptableObject
{
    [SerializeField]
    private ItemStack prefab;

    public ItemStack Prefab
    {
        get { return prefab; }
    }
}