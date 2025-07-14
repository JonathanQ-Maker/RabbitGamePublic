using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class FIMFLoader
{
    private List<Vector3> vertexRegistry = new List<Vector3>();
    private List<Vector2> uvRegistry = new List<Vector2>();
    private List<Vector3> normalRegistry = new List<Vector3>();
    private Dictionary<string, string> groupMap = new Dictionary<string, string>();

    public void Load(StreamReader inputStream, out FIMFModel node, out FIMFAnimation[] animations)
    {
        node = null;
        animations = null;

        List<FIMFAnimation> animationList = new List<FIMFAnimation>();
        string tag;
        string[] split;
        while ((tag = inputStream.ReadLine()) != null)
        {
            split = tag.Split(' ');
            tag = split[0];

            if (tag == "bg")
            {
                node = ParseGroup(inputStream, split, null);
            }

            if (tag == "ba")
            {
                animationList.Add(ParseAnimation(inputStream, split));
            }
        }
        animations = animationList.ToArray();
        Clear();
    }

    private void Clear()
    {
        vertexRegistry.Clear();
        uvRegistry.Clear();
        normalRegistry.Clear();
        groupMap.Clear();
    }

    private FIMFSequence ParseSequence(StreamReader reader, string[] split)
    {
        List<Vector4> positionKeys = new List<Vector4>();
        List<Quaternion> rotationKeys = new List<Quaternion>();
        List<float> rotTimeKeys = new List<float>();
        List<Vector4> scaleKeys = new List<Vector4>();

        string path = groupMap[split[1]];
        string tag;
        while ((tag = reader.ReadLine()) != null)
        {
            split = tag.Split(' ');
            tag = split[0];

            if (tag == "p")
            {
                positionKeys.Add(new Vector4(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]), float.Parse(split[4])));
            }

            if (tag == "r")
            {
                rotationKeys.Add(new Quaternion(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]), float.Parse(split[4])));
                rotTimeKeys.Add(float.Parse(split[5]));
            }

            if (tag == "s")
            {
                scaleKeys.Add(new Vector4(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]), float.Parse(split[4])));
            }

            if (tag == "eas")
            {
                break;
            }
        }
        return new FIMFSequence(path, positionKeys.ToArray(), rotationKeys.ToArray(), rotTimeKeys.ToArray(), scaleKeys.ToArray());
    }

    private FIMFAnimation ParseAnimation(StreamReader reader, string[] split)
    {
        List<FIMFSequence> sequences = new List<FIMFSequence>();
        string animationName = split[1];
        float animationDuration = float.Parse(split[2]);
        string tag;
        while ((tag = reader.ReadLine()) != null)
        {
            split = tag.Split(' ');
            tag = split[0];

            if (tag == "bas")
            {
                sequences.Add(ParseSequence(reader, split));
            }

            if (tag == "ea")
            {
                break;
            }
        }
        return new FIMFAnimation(animationName, animationDuration, sequences.ToArray());
    }

    private Mesh ParseMesh(StreamReader reader, string[] split)
    { 
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        string meshName = split[1];
        string tag;
        while ((tag = reader.ReadLine()) != null)
        {
            split = tag.Split(' ');
            tag = split[0];

            if (tag == "v")
            {
                vertexRegistry.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
            }

            if (tag == "vt")
            {
                uvRegistry.Add(new Vector2(float.Parse(split[1]), float.Parse(split[2])));
            }

            if (tag == "vn")
            {
                normalRegistry.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
            }

            if (tag == "t")
            {
                for (int i = 1; i < 4; i++)
                {
                    string[] refs = split[i].Split("/", 3);
                    int vRef = int.Parse(refs[0]);
                    int vtRef = int.Parse(refs[1]);
                    int vnRef = int.Parse(refs[2]);

                    vertices.Add(vertexRegistry[vRef]);
                    uv.Add(uvRegistry[vtRef]);
                    normals.Add(normalRegistry[vnRef]);
                    triangles.Add(triangles.Count);
                }
            }

            if (tag == "em")
            {
                break;
            }
        }
        return ToMesh(vertices, uv, normals, triangles, meshName);
    }

    private FIMFModel ParseGroup(StreamReader reader, string[] split, string fullGroupPath)
    {
        string name = split[1];
        if (!string.IsNullOrEmpty(fullGroupPath))
        { 
            fullGroupPath = $"{fullGroupPath}/{name}";
        }
        else if (fullGroupPath == null)
        {
            fullGroupPath = "";
        }
        else
        {
            fullGroupPath = name;
        }
            
        groupMap.Add(name, fullGroupPath);

        Vector3 localPosition;
        localPosition.x = float.Parse(split[2]);
        localPosition.y = float.Parse(split[3]);
        localPosition.z = float.Parse(split[4]);

        Quaternion localRotation;
        localRotation.x = float.Parse(split[5]);
        localRotation.y = float.Parse(split[6]);
        localRotation.z = float.Parse(split[7]);
        localRotation.w = float.Parse(split[8]);

        List<FIMFModel> nodes = new List<FIMFModel>();


        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        string tag;
        while ((tag = reader.ReadLine()) != null)
        {
            split = tag.Split(' ');
            tag = split[0];

            if (tag == "bg")
            {
                nodes.Add(ParseGroup(reader, split, fullGroupPath));
            }

            if (tag == "v")
            {
                vertexRegistry.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
            }

            if (tag == "vt")
            {
                uvRegistry.Add(new Vector2(float.Parse(split[1]), float.Parse(split[2])));
            }

            if (tag == "vn")
            {
                normalRegistry.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
            }

            if (tag == "t")
            {
                for (int i = 1; i < 4; i++)
                {
                    string[] refs = split[i].Split("/", 3);
                    int vRef = int.Parse(refs[0]);
                    int vtRef = int.Parse(refs[1]);
                    int vnRef = int.Parse(refs[2]);

                    vertices.Add(vertexRegistry[vRef]);
                    uv.Add(uvRegistry[vtRef]);
                    normals.Add(normalRegistry[vnRef]);
                    triangles.Add(triangles.Count);
                }
            }

            if (tag == "eg")
            {
                break;
            }
        }

        Mesh mesh = null;
        if (triangles.Count > 0)
            mesh = ToMesh(vertices, uv, normals, triangles, name);
        return new FIMFModel(name, localPosition, localRotation, nodes.ToArray(), mesh);
    }

    private static Mesh ToMesh(List<Vector3> vertices, List<Vector2> uv, List<Vector3> normals, List<int> triangles, string name)
    {
        Mesh mesh = new Mesh();
        mesh.name = name;
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.RecalculateNormals();
        mesh.Optimize();
        return mesh;
    }
}
