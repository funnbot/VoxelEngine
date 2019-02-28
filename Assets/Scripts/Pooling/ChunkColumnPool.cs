using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

public class ChunkColumnPool : PrefabPool {
    public override void CleanUp(GameObject go) {
        var col = go.GetComponent<ChunkColumn>();
        col.
    }
}