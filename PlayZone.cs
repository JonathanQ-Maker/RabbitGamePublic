using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class PlayZone : MonoBehaviour
{
    public List<Vector2> baseVertices;

    [Range(1f, 20f)]
    public float height = 5f;

    private void Start()
    {
        UpdateZone();
    }

    public void UpdateZone()
    {
        if (baseVertices.Count < 3) return;

        Mesh boarder = new Mesh();
        Vector3[] vertices = new Vector3[baseVertices.Count * 4];
        int[] triganles = new int[baseVertices.Count * 6];
        if (IsClockWise())
        {
            for (int i = 0; i < baseVertices.Count; ++i)
            {
                //
                // 0--1
                // |  |
                // 2--3
                //
                Vector2 a = baseVertices[i];
                Vector2 b = baseVertices[(i + 1) % baseVertices.Count];
                int vertGroupIdx = i * 4;
                int trigGroupIdx = i * 6;
                vertices[vertGroupIdx] = new Vector3(a.x, height, a.y);
                vertices[vertGroupIdx + 1] = new Vector3(b.x, height, b.y);
                vertices[vertGroupIdx + 2] = new Vector3(a.x, 0, a.y);
                vertices[vertGroupIdx + 3] = new Vector3(b.x, 0, b.y);

                triganles[trigGroupIdx] = vertGroupIdx;
                triganles[trigGroupIdx + 1] = vertGroupIdx + 1;
                triganles[trigGroupIdx + 2] = vertGroupIdx + 2;
                triganles[trigGroupIdx + 3] = vertGroupIdx + 1;
                triganles[trigGroupIdx + 4] = vertGroupIdx + 3;
                triganles[trigGroupIdx + 5] = vertGroupIdx + 2;
            }
        }
        else
        {
            for (int i = 0; i < baseVertices.Count; ++i)
            {
                //
                // 0--1
                // |  |
                // 2--3
                //
                Vector2 a = baseVertices[i];
                Vector2 b = baseVertices[(i + 1) % baseVertices.Count];
                int vertGroupIdx = i * 4;
                int trigGroupIdx = i * 6;
                vertices[vertGroupIdx] = new Vector3(a.x, height, a.y);
                vertices[vertGroupIdx + 1] = new Vector3(b.x, height, b.y);
                vertices[vertGroupIdx + 2] = new Vector3(a.x, 0, a.y);
                vertices[vertGroupIdx + 3] = new Vector3(b.x, 0, b.y);

                triganles[trigGroupIdx] = vertGroupIdx;
                triganles[trigGroupIdx + 1] = vertGroupIdx + 2;
                triganles[trigGroupIdx + 2] = vertGroupIdx + 1;
                triganles[trigGroupIdx + 3] = vertGroupIdx + 1;
                triganles[trigGroupIdx + 4] = vertGroupIdx + 2;
                triganles[trigGroupIdx + 5] = vertGroupIdx + 3;
            }
        }

        boarder.vertices = vertices;
        boarder.triangles = triganles;
        boarder.RecalculateNormals();
        boarder.RecalculateBounds();
        GetComponent<MeshCollider>().sharedMesh = boarder;
    }

    private bool IsClockWise()
    {
        // see: https://stackoverflow.com/a/1180256 and http://en.wikipedia.org/wiki/Curve_orientation
        // Requires vertices.Count >= 3
        int minIdx = 0;
        for (int i = 1; i < baseVertices.Count; ++i)
        {
            if (CompareVertex(baseVertices[i], baseVertices[minIdx]) < 0)
            { 
                minIdx = i;
            }
        }
        Vector2 A = baseVertices[(minIdx + baseVertices.Count - 1) % baseVertices.Count];
        Vector2 B = baseVertices[minIdx];
        Vector2 C = baseVertices[(minIdx + 1) % baseVertices.Count];

        float det = B.x*C.y - B.y*C.x - A.x*C.y + A.y*C.x + A.x*B.y - A.y*B.x;
        return det < 0;
    }

    private int CompareVertex(Vector2 a, Vector2 b)
    {
        if (a.y < b.y)
        {
            return -1;
        }
        else if (a.y > b.y)
        {
            return 1;
        }
        else
        {
            // equal
            if (a.x > b.x)
                return -1;
            else
                return 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3[] points = new Vector3[baseVertices.Count];
        for(int i = 0; i < baseVertices.Count; ++i)
        {
            points[i] = new Vector3(baseVertices[i].x, transform.position.y, baseVertices[i].y);
            Gizmos.DrawLine(points[i], points[i] + Vector3.up * height);
        }
        Gizmos.DrawLineStrip(points, true);

        for (int i = 0; i < baseVertices.Count; ++i)
        {
            points[i].y += height;
        }
        Gizmos.DrawLineStrip(points, true);
    }
}