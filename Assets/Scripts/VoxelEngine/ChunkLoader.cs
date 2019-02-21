using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class ChunkLoader : MonoBehaviour {
        public VoxelWorld world;
        public int range;

        int tick;
        void Start() {
            var chunkPos = world.BlockToChunkPos(Vector3Int.FloorToInt(transform.position));
        }

        void Update() {

        }
    }

}