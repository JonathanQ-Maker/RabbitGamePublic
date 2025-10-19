using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour, IJsonSerializable, ICharacter
{
    private IEnumerator actionLoop;
    private IEnumerator ActionLoop
    {
        get { return actionLoop; }
        set
        {
            if (actionLoop != null)
                StopCoroutine(actionLoop);
            if (value != null)
                actionLoop = AutoCleanUp(value);
            else
                actionLoop = value;

            if (actionLoop != null)
                StartCoroutine(actionLoop);
        }
    }

    private IEnumerator AutoCleanUp(IEnumerator coroutine)
    {
        yield return coroutine;
        ActionLoop = null;
    }


    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    [Range(0f, 10f)]
    private float moveSpeed;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = Mathf.Max(value, 0); }
    }

    [SerializeField]
    [Range(0.1f, 5f)]
    private float gridResolution = 0.1f;
    public float GridResolution
    {
        get { return gridResolution; }
        set { gridResolution = Mathf.Max(value, 0.1f); }
    }

    [SerializeField]
    [Range(5, 50)]
    private int gridSize = 5;
    public int GridSize 
    {
        get { return gridSize; }
        set { gridSize = Mathf.Max(value, 5); }
    }

    private IMountable mount;
    public bool Mounted {
        get { return !ReferenceEquals(mount, null); }
    }

    private ColliderInformer colliderInformer;

    private delegate void OnOpen(object result);
    private delegate void OnClose();

    private IOpenable openedObject;
    public object OpenedObject => openedObject;
    private OnOpen onOpenObject;
    private OnClose onCloseObject;

    private SimpleInventory inventory;
    public SimpleInventory Inventory { get { return inventory; } }


    private void Awake()
    {
        colliderInformer = new ColliderInformer(new Vector3(0.75f, 1.5f, 0.75f), 65);
        inventory = new SimpleInventory(40);
    }

    /////////////////////////////////////////
    // "Start" Actions
    /////////////////////////////////////////

    public void StartMoveTo(Vector3 target)
    {
        GetPath(target, (PathResult result) => {
            pathDebug.Result = result;
            if (result.Length > 0)
            {
                ActionLoop = TraversePath(result.Path, true);
            }
        });
    }

    public void StartLookAt(Vector3 target)
    {
        ActionLoop = RigidBodyMover.LookAt(rb, target);
    }

    public void StartUse(IUsable usable)
    {
        if (usable is Component component)
        { 
            GetPath(component.transform.position, (PathResult result) => {
                pathDebug.Result = result;
                if (result.Length > 0)
                {
                    ActionLoop = UseAt(result.Path, component.transform, usable);
                }
            });
            return;
        }
        
        usable.Use();
    }

    public void StartOpen(IOpenable openable)
    {
        if (openable is Component component)
        { 
            GetPath(component.transform.position, (PathResult result) => {
                pathDebug.Result = result;
                if (result.Length > 0)
                {
                    ActionLoop = OpenAt(result.Path, component.transform, openable);
                }
            });
            return;
        }

        onOpenObject?.Invoke(openable.Open(this));
    }

    public void StartMount(IMountable mountable)
    {
        GetPath(mountable.Position, (PathResult result) => {
            pathDebug.Result = result;
            if (result.Length > 0)
            {
                ActionLoop = Mount(result.Path, mountable);
            }
        });
    }

    public void StartGetItem(WorldItemRenderer worldItem) 
    {
        GetPath(worldItem.transform.position, (PathResult result) => {
            pathDebug.Result = result;
            if (result.Length > 0)
            {
                ActionLoop = GetItem(result.Path, worldItem);
            }
        });
    }

    public void DisMount()
    {
        if (Mounted)
        {
            mount.OnDismount(transform);
            mount = null;
            rb.isKinematic = false;
            SetTrigger("Idle");
        }
    }

    public void CloseContainer()
    {
        if (!ReferenceEquals(OpenedObject, null)) 
        {
            openedObject.Close();
            openedObject = null;
            onCloseObject?.Invoke();
        }
    }









    /////////////////////////////////////////
    // Public Functions
    /////////////////////////////////////////
    public void Deserialize(JObject data)
    {
        
    }

    public void Serialize(JObject data)
    {
        
    }

    public void Subscribe(ICharacterController controller) 
    {
        onOpenObject += controller.OnOpen;
        onCloseObject += controller.OnClose;
    }

    public void Unsubscribe(ICharacterController controller)
    {
        onOpenObject -= controller.OnOpen;
        onCloseObject -= controller.OnClose;
    }






    /////////////////////////////////////////
    // Private Helper Functions
    /////////////////////////////////////////
    private void GetPath(Vector3 target, Action<PathResult> callback)
    {
        Vector3 start = transform.position;
        start.y = 1;
        colliderInformer.Bake(start, GridSize, GridResolution);
        PathRequestManager.RequestPath(
            new PathRequest(start, target, GridSize, GridResolution, colliderInformer),
                callback);
    }

    private IEnumerator TraversePath(Vector3[] path, bool inclusive)
    {
        if (path.Length == 0 || Mounted) yield break;
        SetTrigger("Walk");
        yield return RigidBodyMover.TraversePath(rb, path, MoveSpeed, inclusive);
        animator.SetTrigger("Idle");
    }

    private IEnumerator UseAt(Vector3[] path, Transform at, IUsable target)
    {
        if (Vector3.Distance(at.position, transform.position) > 1.5f)
            yield return TraversePath(path, false);

        if (CheckDestroyed(at)) yield break;

        yield return RigidBodyMover.LookAt(rb, at.position);

        if (CheckDestroyed(at)) yield break;
        if (Vector3.Distance(at.position, transform.position) < 2f)
        {
            SetTrigger("Use");
            target.Use();
        }
    }

    private IEnumerator OpenAt(Vector3[] path, Transform at, IOpenable target)
    {
        if (Vector3.Distance(at.position, transform.position) > 1.5f)
            yield return TraversePath(path, false);

        if (CheckDestroyed(at)) yield break;

        yield return RigidBodyMover.LookAt(rb, at.position);

        if (CheckDestroyed(at)) yield break;
        if (Vector3.Distance(at.position, transform.position) < 2f)
        {
            SetTrigger("Use");
            onOpenObject?.Invoke(target.Open(this));
            openedObject = target;
        }
    }

    private IEnumerator GetItem(Vector3[] path, WorldItemRenderer worldItem)
    {
        if (Vector3.Distance(worldItem.transform.position, transform.position) > 1.5f)
            yield return TraversePath(path, false);

        if (CheckDestroyed(worldItem)) yield break;

        yield return RigidBodyMover.LookAt(rb, worldItem.transform.position);

        if (CheckDestroyed(worldItem)) yield break;
        if (Vector3.Distance(worldItem.transform.position, transform.position) < 2f)
        {
            SetTrigger("Use");
            // TODO: Pickup item
            if (worldItem.ItemStack.Count == inventory.AddItem(worldItem.ItemStack))
            {
                Destroy(worldItem.gameObject);
            }
        }
    }

    private IEnumerator Mount(Vector3[] path, IMountable target)
    {
        if (Vector3.Distance(target.Position, transform.position) > 1.5f)
            yield return TraversePath(path, false);

        if (CheckDestroyed(target)) yield break;
        yield return RigidBodyMover.LookAt(rb, target.Position);

        if (CheckDestroyed(target)) yield break;
        if (Vector3.Distance(target.Position, transform.position) < 3f)
        {
            if (target.CanMount)
            {
                if (Mounted)
                {
                    mount.OnDismount(transform);
                }
                if (target.OnMount(transform))
                {
                    SetTrigger("Sitting");
                    mount = target;
                    rb.isKinematic = true;
                }
            }
        }
    }

    private void SetTrigger(string trigger)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
        animator.SetTrigger(trigger);
    }

    private static bool CheckDestroyed(object obj)
    {
        if (obj is MonoBehaviour)
            return obj as MonoBehaviour == null;
        return obj == null;
    }

 






















    /////////////////////////////////////////
    // Debug
    /////////////////////////////////////////
    [SerializeField]
    private PathFindingDebug pathDebug;
}