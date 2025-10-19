using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Rabbit : MonoBehaviour, IJsonSerializable, ICharacter
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

    public SimpleInventory Inventory { get { return null; } }

    private object openedObject;
    public object OpenedObject => openedObject;
    public bool Mounted { get { return false; } }


    private ColliderInformer colliderInformer;

    private void Awake()
    {
        colliderInformer = new ColliderInformer(new Vector3(0.75f, 0.5f, 0.75f), 65);
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
            StartMoveTo(component.transform.position);
            return;
        }

        usable.Use();
    }

    public void StartOpen(IOpenable openable)
    {
        if (openable is Component component)
        {
            StartMoveTo(component.transform.position);
            return;
        }
    }

    public void CloseContainer() 
    {

    }

    public void StartMount(IMountable mountable)
    { 
    
    }

    public void DisMount()
    { 
    
    }

    public void StartGetItem(WorldItemRenderer worldItem)
    {

    }

    public void Subscribe(ICharacterController controller)
    { 

    }

    public void Unsubscribe(ICharacterController controller)
    {

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























    /////////////////////////////////////////
    // Private Helper Functions
    /////////////////////////////////////////
    private void GetPath(Vector3 target, Action<PathResult> callback)
    {
        Vector3 start = transform.position;
        start.y = 0.5f;
        colliderInformer.Bake(start, GridSize, GridResolution);
        PathRequest request = new PathRequest(start, target, GridSize, GridResolution, colliderInformer);
        PathRequestManager.RequestPath(request, callback);
    }

    private IEnumerator TraversePath(Vector3[] path, bool inclusive)
    {
        if (path.Length == 0) yield break;
        SetTrigger("Walk");
        yield return RigidBodyMover.TraversePath(rb, path, MoveSpeed, inclusive);
        SetTrigger("Idle");
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

 




























    /////////////////////////////////////////
    // Debug
    /////////////////////////////////////////

    [SerializeField]
    private PathFindingDebug pathDebug;
}