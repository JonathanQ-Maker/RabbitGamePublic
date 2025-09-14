using UnityEngine;

public interface IWorldUsable
{
    Vector3 Position { get; }

    void Use();
}