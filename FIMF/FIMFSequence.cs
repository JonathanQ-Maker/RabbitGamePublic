using System;
using UnityEngine;
using System.Collections;


public class FIMFSequence
{
    private string targetPath; // full path
    private Vector4[] positionKeys;
    private Quaternion[] rotationKeys;
    private float[] rotTimeKeys;
    private Vector4[] scaleKeys;


    public string TargetPath
    {
        get { return targetPath; }
    }

    public Vector4[] PositionKeys
    { 
        get { return positionKeys; } 
    }

    public Quaternion[] RotationKeys 
    { 
        get { return rotationKeys; } 
    }

    public float[] RotTimeKeys
    {
        get { return rotTimeKeys; }
    }

    public Vector4[] ScaleKeys
    {
        get { return scaleKeys; }
    }

    public FIMFSequence(string targetPath, Vector4[] positionKeys, Quaternion[] rotationKeys, float[] rotTimeKeys, Vector4[] scaleKeys)
    {
        this.targetPath = targetPath;
        this.positionKeys = positionKeys;
        this.scaleKeys = scaleKeys;
        this.rotationKeys = rotationKeys;
        this.rotTimeKeys = rotTimeKeys;

        Vec4TimeComparer timeComparer = new Vec4TimeComparer();
        Array.Sort(positionKeys, timeComparer);
        Array.Sort(scaleKeys, timeComparer);
        Array.Sort(rotTimeKeys, rotationKeys);
    }


    private class Vec4TimeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((Vector4)x).w < ((Vector4)y).w ? -1 : 1;
        }
    }
}
