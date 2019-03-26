using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class ChunkManager {
        public delegate void ChunkUpdate();
        public event ChunkUpdate OnChunkUpdate;

        private Dictionary<Coord2, Chunk> chunks;

        public ChunkManager() {
            chunks = new Dictionary<Coord2, Chunk>();
        }
    }

}