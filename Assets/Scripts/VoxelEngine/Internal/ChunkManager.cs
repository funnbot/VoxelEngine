using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Pooling;

namespace VoxelEngine.Internal {

    public class ChunkManager {
        public delegate void ChunkUpdate();
        public event ChunkUpdate OnChunkUpdate;

        private Dictionary<Coord2, Chunk> chunks;
        private ChunkPool pool;

        public ChunkManager(ChunkPool pool) {
            chunks = new Dictionary<Coord2, Chunk>();
            this.pool = pool;
        }

        public void UpdateChunks() {
            OnChunkUpdate?.Invoke();
        }

        public Chunk LoadChunk(Coord2 coord) {
            var chunk = pool.GetObject();
            chunk.Setup(coord);
            chunks.Add(coord, chunk);
            return chunk;
        }

        public void DestroyChunk(Coord2 coord) {
            Chunk chunk = GetChunk(coord);
            pool.ReleaseObject(chunk);
            chunks.Remove(coord);
        }

        public Chunk GetChunk(Coord2 coord) =>
            chunks.GetValueOrDefault(coord);

        public bool ContainsChunk(Coord2 coord) =>
            chunks.ContainsKey(coord);

        public ChunkSection GetSection(Coord3 position) =>
            GetChunk((Coord2) position)?.GetSection(position.y);

    }

}