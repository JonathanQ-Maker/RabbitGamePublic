using UnityEngine;

public class Cabin : MonoBehaviour, ISimpleContainer
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

    public void OnOpenSimpleInv(object source)
    {
        animator.SetTrigger("Open");
    }

    public void OnCloseSimpleInv(object source)
    {
        animator.SetTrigger("Close");
    }
}