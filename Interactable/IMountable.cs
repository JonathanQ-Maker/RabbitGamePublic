using UnityEngine;

public interface IMountable
{
    Vector3 Position { get; }

    bool CanMount { get; }
    bool OnMount(Transform mountee);
    void OnDismount(Transform mountee);
}