using UnityEngine;

public interface IUsable
{
    Vector3 Position { get; }

    void Use();
}