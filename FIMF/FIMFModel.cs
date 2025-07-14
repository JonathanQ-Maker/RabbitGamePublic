using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FIMFModel : IEnumerable<FIMFModel>
{
    private FIMFModel[] childs;
    private Mesh mesh;
    private Vector3 localPosition;
    private Quaternion localRotation;
    private string name;
    private FIMFAnimation[] animations;

    public Vector3 LocalPosition { get { return localPosition; } }
    public Quaternion LocalRotation { get { return localRotation; } }
    public string Name { get { return name; } }


    public Mesh Mesh { get { return mesh; } }

    public FIMFModel GetChild(int i) { return childs[i]; }
    public int ChildCount { get { return childs.Length; } }

    public FIMFModel(string name, Vector3 localPosition, Quaternion localRotation, FIMFModel[] childs, Mesh mesh)
    { 
        this.name = name;
        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.childs = childs;
        this.mesh = mesh;
    }

    public GameObject ToGameObject(Transform parent, Material material)
    {
        GameObject root = new GameObject(name);
        root.transform.SetLocalPositionAndRotation(localPosition, localRotation);
        root.transform.SetParent(parent, false);

        // mesh
        if (mesh != null)
        {
            MeshFilter filter = root.AddComponent<MeshFilter>();
            MeshRenderer renderer = root.AddComponent<MeshRenderer>();
            filter.sharedMesh = mesh;
            renderer.sharedMaterial = material;
        }
            

        foreach (FIMFModel child in childs)
        {
            child.ToGameObject(root.transform, material);
        }
        return root;
    }

    public IEnumerator<FIMFModel> GetEnumerator()
    {
        return ((IEnumerable<FIMFModel>)childs).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return childs.GetEnumerator();
    }
}
