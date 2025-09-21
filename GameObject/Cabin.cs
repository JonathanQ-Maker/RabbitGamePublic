using UnityEngine;

public class Cabin : MonoBehaviour, IOpenable
{
    public Vector3 Position => transform.position;

    [SerializeField]
    private Animator animator;

    private SimpleInventory inventory;

    public SimpleInventory SimpleInv => inventory;

    private void Start()
    {
        inventory = new SimpleInventory(30);
    }

    public object Open(object source)
    {
        animator.SetTrigger("Open");
        return inventory;
    }

    public void Close()
    {
        animator.SetTrigger("Close");
    }
}