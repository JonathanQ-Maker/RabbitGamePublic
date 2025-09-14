using UnityEngine;

public class Seat : MonoBehaviour, IMountable
{
    [SerializeField]
    private Transform dismount, mount;
    public Vector3 Position => transform.position;

    public bool CanMount
    {
        get { return mount.childCount == 0; }
    }

    public void OnDismount(Transform mountee)
    {
        foreach (Transform child in mount)
        {
            if (ReferenceEquals(child, mountee))
            {
                child.SetParent(null);
                child.SetLocalPositionAndRotation(dismount.position, Quaternion.identity);
                return;
            }
        }
    }

    public bool OnMount(Transform mountee)
    {
        if (!CanMount) return false;

        mountee.SetParent(mount, false);
        mountee.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        return true;
    }
}