using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float maxFocusSpeed = 30, smoothTime = 0.05f, maxCameraSize = 10f;

    public void StartFollow(Transform target)
    {
        ActionLoop = Follow(target);
    }

    public void StartFocus(Transform target)
    {
        ActionLoop = Focus(target.position);
    }

    public void StartFocus(Vector3 pos)
    {
        ActionLoop = Focus(pos);
    }

    private void Update()
    {
        HandleMovement();
        HandleScroll();
    }

    private Camera attachedCamera;

    private void Start()
    {
        attachedCamera = GetComponent<Camera>();
    }

    private Vector3 dragStartPosition;
    private IEnumerator actionLoop;
    private IEnumerator ActionLoop
    {
        get { return actionLoop; }
        set
        {
            if (actionLoop != null)
            {
                StopCoroutine(actionLoop);
            }
            actionLoop = value;
            if (actionLoop != null)
                StartCoroutine(actionLoop);
        }
    }

    private Plane plane = new Plane(Vector3.up, Vector3.zero);

    private IEnumerator Follow(Transform target)
    {
        float entry;
        Vector3 velocity = Vector3.zero;
        Vector2 screenMid = new Vector2(Screen.width / 2, Screen.height / 2);
        while (true)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenMid);
            if (plane.Raycast(ray, out entry))
            {
                Vector3 currentPos = ray.GetPoint(entry);
                Vector3 targetPos = target.position;
                targetPos.y = 0;
                currentPos.y = 0;


                Vector3 nextPos = Vector3.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime, maxFocusSpeed);
                Vector3 delta = nextPos - currentPos;
                transform.position += delta;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Focus(Vector3 pos)
    {
        // TODO: allow focus in y axis
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        Vector3 initalCamPos = transform.position;
        Vector3 targetDelta = Vector3.zero;
        Vector3 currentDelta = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        float entry;
        if (plane.Raycast(ray, out entry))
        {
            targetDelta = pos - ray.GetPoint(entry);
            targetDelta.y = 0;
        }

        while ((targetDelta - currentDelta).magnitude > 0.1f)
        {
            currentDelta = Vector3.SmoothDamp(currentDelta, targetDelta, ref velocity, smoothTime, maxFocusSpeed);
            transform.position = initalCamPos + currentDelta;
            yield return new WaitForFixedUpdate();
        }
        transform.position = initalCamPos + targetDelta;
    }

    private void HandleMovement()
    {
        // Drag code from https://youtu.be/rnqF6S7PfFA?t=756

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
            ActionLoop = null;
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray, out entry))
            {
                transform.position += dragStartPosition - ray.GetPoint(entry);
            }
        }
    }

    private void HandleScroll()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            Vector2 view = attachedCamera.ScreenToViewportPoint(Input.mousePosition);
            bool isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;

            if (!isOutside)
            {
                // subtract because controls are inverted
                float size = attachedCamera.orthographicSize - Input.mouseScrollDelta.y;
                attachedCamera.orthographicSize = Mathf.Clamp(size, 1, maxCameraSize);
            }
        }
    }
}
