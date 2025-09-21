using UnityEngine;

public class VendingMachine : MonoBehaviour, IOpenable
{
    public Vector3 Position => transform.position;

    [SerializeField]
    private Animator animator;
    private SimpleInventory inventory;

    private void Start()
    {
        inventory = new SimpleInventory(5);
    }

    public object Open(object source)
    {
        animator.SetTrigger("HatchFlip");
        return inventory;
    }

    public void Close()
    {
    }
}