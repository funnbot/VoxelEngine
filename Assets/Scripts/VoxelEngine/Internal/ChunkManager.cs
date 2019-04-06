using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Pooling;

namespace VoxelEngine.Internal {

    public class ChunkManager {
        private Dictionary<Coord2, Chunk> chunks;
        private ChunkPool pool;

        public ChunkManager(ChunkPool pool) {
            this.pool = pool;

            chunks = new Dictionary<Coord2, Chunk>();
        }

        /// Gets a chunk from the pool and initializes it
        public Chunk CreateChunk(Coord2 coord) {
            var chunk = GetChunk(coord);
            if (chunk != null) return chunk;

            chunk = pool.GetObject();
            chunk.Setup(coord);
            chunks.Add(coord, chunk);
            return chunk;
        }

        public void DeleteChunk(Chunk chunk) {
            chunks.Remove(chunk.position);
            pool.ReleaseObject(chunk);
        }

        public Chunk GetChunk(Coord2 coord) =>
            chunks.GetValueOrDefault(coord);

        public bool ContainsChunk(Coord2 coord) =>
            chunks.ContainsKey(coord);

        public ChunkSection GetSection(Coord3 position) =>
            GetChunk((Coord2) position)?.GetSection(position.y);
    }
}