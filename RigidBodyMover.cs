using System.Collections;
using UnityEngine;

public static class RigidBodyMover
{
    public static IEnumerator MoveTo(Rigidbody rigidbody, Vector3 targetPos, float maxSpeed)
    {
        targetPos.y = rigidbody.transform.position.y;
        Vector3 velocity = Vector3.zero;
        float angleVelocity = 0;
        Vector3 delta = targetPos - rigidbody.transform.position;
        if (delta.sqrMagnitude < 0.1f) yield break;

        float angle = Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;
        int i = 0;
        while (delta.sqrMagnitude > 0.01f || (Mathf.Abs(Mathf.DeltaAngle(angle, rigidbody.transform.eulerAngles.y)) > 10f))
        {
            if (i >= 200) break;
            Vector3 position = Vector3.SmoothDamp(rigidbody.transform.position, targetPos, ref velocity, 0.1f, maxSpeed);
            Quaternion rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(rigidbody.transform.eulerAngles.y, angle, ref angleVelocity, 0.1f), 0);
            rigidbody.Move(position, rotation);
            delta = targetPos - rigidbody.transform.position;
            i++;
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator LookAt(Rigidbody rigidbody, Vector3 targetPos)
    {
        Vector3 tPos = new Vector3(targetPos.x, rigidbody.transform.position.y, targetPos.z);
        Vector3 delta = tPos - rigidbody.transform.position;
        float angleVelocity = 0;
        float angle = Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(angle, rigidbody.transform.eulerAngles.y)) > 10f)
        {
            rigidbody.MoveRotation(Quaternion.Euler(0, Mathf.SmoothDampAngle(rigidbody.transform.eulerAngles.y, angle, ref angleVelocity, 0.1f), 0));
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator TraversePath(Rigidbody rigidbody, Vector3[] path, float maxSpeed, bool inclusive)
    {
        if (path.Length <= 0) yield break;
        for (int i = 0; i < path.Length - 1; ++i)
        {
            yield return MoveTo(rigidbody, path[i], maxSpeed);
        }

        // Path finder ignores walkabilty on the end node such that it can support entities
        // pathing to block rather than just pathing to empty space.
        if (inclusive)
            yield return MoveTo(rigidbody, path[path.Length - 1], maxSpeed);
    }
}