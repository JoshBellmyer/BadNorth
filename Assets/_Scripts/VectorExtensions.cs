using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3Int GetRotation90(this Vector3Int v, Vector3Int axis)
    {
        if (axis != Vector3Int.up) throw new System.NotImplementedException("I'm too lazy to implement this");
        return new Vector3Int(v.z, v.y, -v.x);
    }
}
