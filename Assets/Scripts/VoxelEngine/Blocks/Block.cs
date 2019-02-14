using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class Block {
        public BlockData data;
        public Vector3Int position;
        public Vector2Int rotation;

        public Block(BlockData data, Vector3Int position = new Vector3Int(), Vector2Int rotation = new Vector2Int()) {
            this.data = data;
            this.position = position;
            this.rotation = rotation;
        }
    }
    
}