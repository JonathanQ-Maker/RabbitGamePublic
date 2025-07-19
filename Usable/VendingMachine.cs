using UnityEngine;

public class VendingMachine : MonoBehaviour, IUsable
{
    public Vector3 Position => transform.position;

    public void Use()
    {
        
    }
}