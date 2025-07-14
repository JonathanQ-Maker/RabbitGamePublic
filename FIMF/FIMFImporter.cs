using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEditor.UIElements;
using UnityEngine;

[ScriptedImporter(1, "fimf")]
public class FIMFImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        FIMFLoader loader = new FIMFLoader();

        using (StreamReader reader = new StreamReader(ctx.assetPath))
        {
            loader.Load(reader, out FIMFModel model, out FIMFAnimation[] anims);

            // GameObject
            GameObject gameObject = model.ToGameObject(null, null);
            ctx.AddObjectToAsset(model.Name, gameObject);

            // Meshes
            MeshFilter[] filters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter filter in filters)
            {
                ctx.AddObjectToAsset(filter.sharedMesh.name, filter.sharedMesh);
            }

            // Animations
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            string[] paths = new string[transforms.Length];
            for (int i = 0; i < transforms.Length; ++i)
            {
                paths[i] = GetPath(transforms[i]);
            }
            foreach (FIMFAnimation anim in anims)
            {
                HashSet<string> closedPaths= new HashSet<string>();
                AnimationClip animationClip = new AnimationClip();
                foreach (FIMFSequence seq in anim)
                {
                    ParseFIMFSeq(animationClip, seq, gameObject.transform.Find(seq.TargetPath), anim.Duration);
                    closedPaths.Add(seq.TargetPath);
                }
                for (int i = 0; i < paths.Length; ++i)
                {
                    if (closedPaths.Contains(paths[i]))
                        continue;

                    closedPaths.Add(paths[i]);

                    // non-initalized object
                    AddInitalCurve(animationClip, paths[i], transforms[i]);
                }
                animationClip.name = anim.Name;
                ctx.AddObjectToAsset(anim.Name, animationClip);
            }
        }
    }

    private void ParseFIMFSeq(AnimationClip clip, FIMFSequence seq, Transform inital, float totalDuration)
    {
        // position keys
        AnimationCurve posX = new AnimationCurve();
        AnimationCurve posY = new AnimationCurve();
        AnimationCurve posZ = new AnimationCurve();

        // rotation keys
        AnimationCurve rotX = new AnimationCurve();
        AnimationCurve rotY = new AnimationCurve();
        AnimationCurve rotZ = new AnimationCurve();
        AnimationCurve rotW = new AnimationCurve();

        // scale keys
        AnimationCurve scaleX = new AnimationCurve();
        AnimationCurve scaleY = new AnimationCurve();
        AnimationCurve scaleZ = new AnimationCurve();


        if (seq.PositionKeys.Length == 0)
        {
            posX.AddKey(0, inital.localPosition.x);
            posY.AddKey(0, inital.localPosition.y);
            posZ.AddKey(0, inital.localPosition.z);

            // adding duration keeping key
            posX.AddKey(totalDuration, inital.localPosition.x);
        }
        else if (seq.PositionKeys[seq.PositionKeys.Length - 1].w < totalDuration)
        {
            posX.AddKey(totalDuration, seq.PositionKeys[seq.PositionKeys.Length - 1].x);
        }
        foreach (Vector4 posKey in seq.PositionKeys)
        {
            posX.AddKey(posKey.w, posKey.x);
            posY.AddKey(posKey.w, posKey.y);
            posZ.AddKey(posKey.w, posKey.z);
        }


        if (seq.ScaleKeys.Length == 0)
        {
            scaleX.AddKey(0, inital.localScale.x);
            scaleY.AddKey(0, inital.localScale.y);
            scaleZ.AddKey(0, inital.localScale.z);
        }
        foreach (Vector4 scaleKey in seq.ScaleKeys)
        {
            scaleX.AddKey(scaleKey.w, scaleKey.x);
            scaleY.AddKey(scaleKey.w, scaleKey.y);
            scaleZ.AddKey(scaleKey.w, scaleKey.z);
        }


        if (seq.RotTimeKeys.Length == 0)
        { 
            rotX.AddKey(0, inital.localRotation.x);
            rotY.AddKey(0, inital.localRotation.y);
            rotZ.AddKey(0, inital.localRotation.z);
            rotW.AddKey(0, inital.localRotation.w);
        }
        for (int i = 0; i < seq.RotTimeKeys.Length; ++i)
        {
            rotX.AddKey(seq.RotTimeKeys[i], seq.RotationKeys[i].x);
            rotY.AddKey(seq.RotTimeKeys[i], seq.RotationKeys[i].y);
            rotZ.AddKey(seq.RotTimeKeys[i], seq.RotationKeys[i].z);
            rotW.AddKey(seq.RotTimeKeys[i], seq.RotationKeys[i].w);
        }

        clip.SetCurve(seq.TargetPath, typeof(Transform), "localPosition.x", posX);
        clip.SetCurve(seq.TargetPath, typeof(Transform), "localPosition.y", posY);
        clip.SetCurve(seq.TargetPath, typeof(Transform), "localPosition.z", posZ);

        clip.SetCurve(seq.TargetPath, typeof(Transform), "localRotation.x", rotX);
        clip.SetCurve(seq.TargetPath, typeof(Transform), "localRotation.y", rotY);
        clip.SetCurve(seq.TargetPath, typeof(Transform), "localRotation.z", rotZ);
        clip.SetCurve(seq.TargetPath, typeof(Transform), "localRotation.w", rotW);

        clip.SetCurve(seq.TargetPath, typeof(Transform), "localScale.x", scaleX);
        clip.SetCurve(seq.TargetPath, typeof(Transform), "localScale.y", scaleY);
        clip.SetCurve(seq.TargetPath, typeof(Transform), "localScale.z", scaleZ);
    }

    private void AddInitalCurve(AnimationClip clip, string targetPath, Transform inital)
    {
        AnimationCurve posX = new AnimationCurve(new Keyframe(0, inital.localPosition.x));
        AnimationCurve posY = new AnimationCurve(new Keyframe(0, inital.localPosition.y));
        AnimationCurve posZ = new AnimationCurve(new Keyframe(0, inital.localPosition.z));

        AnimationCurve rotX = new AnimationCurve(new Keyframe(0, inital.localRotation.x));
        AnimationCurve rotY = new AnimationCurve(new Keyframe(0, inital.localRotation.y));
        AnimationCurve rotZ = new AnimationCurve(new Keyframe(0, inital.localRotation.z));
        AnimationCurve rotW = new AnimationCurve(new Keyframe(0, inital.localRotation.w));

        AnimationCurve scaleX = new AnimationCurve(new Keyframe(0, inital.localScale.x));
        AnimationCurve scaleY = new AnimationCurve(new Keyframe(0, inital.localScale.y));
        AnimationCurve scaleZ = new AnimationCurve(new Keyframe(0, inital.localScale.z));

        clip.SetCurve(targetPath, typeof(Transform), "localPosition.x", posX);
        clip.SetCurve(targetPath, typeof(Transform), "localPosition.y", posY);
        clip.SetCurve(targetPath, typeof(Transform), "localPosition.z", posZ);

        clip.SetCurve(targetPath, typeof(Transform), "localRotation.x", rotX);
        clip.SetCurve(targetPath, typeof(Transform), "localRotation.y", rotY);
        clip.SetCurve(targetPath, typeof(Transform), "localRotation.z", rotZ);
        clip.SetCurve(targetPath, typeof(Transform), "localRotation.w", rotW);

        clip.SetCurve(targetPath, typeof(Transform), "localScale.x", scaleX);
        clip.SetCurve(targetPath, typeof(Transform), "localScale.y", scaleY);
        clip.SetCurve(targetPath, typeof(Transform), "localScale.z", scaleZ);
    }

    private string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return GetPath(current.parent) + "/" + current.name;
    }
}