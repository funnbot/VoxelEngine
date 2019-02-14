using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    public static Vector3 Add(this Vector3 v1, Vector3 v2) {
        v1.x += v2.x;
        v1.y += v2.y;
        v1.z += v2.z;
        return v1;
    }

    public static int MaxElem(this Vector3Int v) {
        return Mathf.Max(Mathf.Max(v.x, v.y), v.z);
    }
    public static int MinElem(this Vector3Int v) {
        return Mathf.Min(Mathf.Min(v.x, v.y), v.z);
    }

    public static Vector3Int Div(this Vector3Int v, float f) {
        v.x = Mathf.FloorToInt(v.x / f);
        v.y = Mathf.FloorToInt(v.y / f);
        v.z = Mathf.FloorToInt(v.z / f);
        return v;
    }
    public static Vector3Int Div(this Vector3Int v1, Vector3Int v2) {
        v1.x /= v2.x;
        v1.y /= v2.y;
        v1.z /= v2.z;
        return v1;
    }
}