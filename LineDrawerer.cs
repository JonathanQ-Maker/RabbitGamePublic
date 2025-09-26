using UnityEngine;

public class LineDrawerer : MonoBehaviour
{
    public Material lineMaterial;
    private int instanceId = -1;
    private Mesh mesh;
    private Vector3 position;
    private Quaternion quaternion;

    public void UpdateBounds(Collider collider)
    {
        if (collider == null) 
        {
            mesh = null;
            instanceId = -1;
            return;
        }

        if (collider is BoxCollider box)
        {
            if (mesh == null || collider.GetInstanceID() != instanceId) 
            { 
                mesh = CreateBoundsOutlineMesh(new Bounds(box.center, box.size * 0.5f));
                instanceId = collider.GetInstanceID();
            }

            position = box.transform.position;
            quaternion = box.transform.rotation;
        }
        else
        {
            if (mesh == null || collider.GetInstanceID() != instanceId) 
            { 
                mesh = CreateBoundsOutlineMesh(new Bounds(Vector3.zero, collider.bounds.extents));
                instanceId = collider.GetInstanceID();
            }

            position = collider.bounds.center;
            quaternion = Quaternion.identity;
        }
    }

    private Mesh CreateBoundsOutlineMesh(Bounds box)
    {
        Mesh mesh = new Mesh();
        mesh.name = "BoxColliderOutline";

        Vector3 c = box.center;
        Vector3 e = box.size;

        // local-space corners
        Vector3[] vertices = new Vector3[8]
        {
            c + new Vector3(-e.x, -e.y, -e.z), // 0
            c + new Vector3(e.x, -e.y, -e.z),  // 1
            c + new Vector3(e.x, -e.y, e.z),   // 2
            c + new Vector3(-e.x, -e.y, e.z),  // 3
            c + new Vector3(-e.x, e.y, -e.z),  // 4
            c + new Vector3(e.x, e.y, -e.z),   // 5
            c + new Vector3(e.x, e.y, e.z),    // 6
            c + new Vector3(-e.x, e.y, e.z)    // 7
        };

        int[] indices = new int[]
        {
            0,1, 1,2, 2,3, 3,0,
            4,5, 5,6, 6,7, 7,4,
            0,4, 1,5, 2,6, 3,7
        };

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        return mesh;
    }

    private void Update()
    {
        if (mesh != null)
            Graphics.DrawMesh(mesh, position, quaternion, lineMaterial, 0);
    }
}