using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

public class ChunkColumnPool : PrefabPool<ChunkColumn> {
    public override void CleanUp(ChunkColumn col) {
        col.CleanUp();
        col.transform.parent = transform;
    }

    public override ChunkColumn Create() {
        var col = Instantiate(prefab).GetComponent<ChunkColumn>();
        col.Create(WorldManager.ActiveWorld);
        col.transform.parent = transform;
        return col;
    }
}