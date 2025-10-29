using UnityEngine;

public interface ICharacter
{
    bool Mounted { get; }
    SimpleInventory Inventory { get; }
    object OpenedObject { get; }
    GameObject gameObject { get; }

    void StartMoveTo(Vector3 target);
    void StartLookAt(Vector3 target);
    void StartUse(IUsable usable);
    void StartOpen(IOpenable openable);
    void CloseContainer();
    void StartMount(IMountable mountable);
    void DisMount();
    void StartGetItem(WorldItemRenderer worldItem);
    void Subscribe(ICharacterController controller);
    void Unsubscribe(ICharacterController controller);
}