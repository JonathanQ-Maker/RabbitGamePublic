using UnityEngine;

public interface ISimpleContainer
{
    Vector3 Position { get; }

    SimpleInventory SimpleInv { get; }

    void OnOpenSimpleInv(object source);

    void OnCloseSimpleInv(object source);
}