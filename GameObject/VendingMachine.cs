using UnityEngine;

public class VendingMachine : MonoBehaviour, IWorldUsable
{
    public Vector3 Position => transform.position;

    [SerializeField]
    private Animator animator;

    public void Use()
    {
        animator.SetTrigger("HatchFlip");
    }
}