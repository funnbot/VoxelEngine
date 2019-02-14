using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {
    public class BlockBehaviour<T> where T : Block {
        protected VoxelWorld world;
        protected Dictionary<Vector3Int, T> blocks;

        public BlockBehaviour<T> Awake(VoxelWorld world) {
            this.world = world;
            blocks = new Dictionary<Vector3Int, T>();
            return this;
        }

        public virtual void Add(Vector3Int pos, T block) {
            blocks.Add(pos, block);
        }

        public virtual void Remove(Vector3Int pos) {
            blocks.Remove(pos);
        }

        public virtual void UnloadChunk(Vector3Int chunkPos) {
            foreach (var block in blocks) {
                var pos = world.BlockToChunkPos(block.Value.position);
                if (chunkPos == pos) Remove(block.Value.position);
            }
        }

        public virtual void OnTick() {

        }

        public virtual void AttachToEvents() {
            world.OnTick += OnTick;
        }
    }

    public class FenceBlockBehaviour<T> {

    }
}