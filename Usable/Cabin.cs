using UnityEngine;

public class Cabin : MonoBehaviour, IUsable
{
    public Vector3 Position => transform.position;

    [SerializeField]
    private Animator animator;
    private bool isOpen = false;
    public bool IsOpen { get { return isOpen; } }

    public void Use()
    {
        animator.SetTrigger(IsOpen ? "Close" : "Open");
        isOpen = !isOpen;
    }
}