using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {
    public class BlockBehaviour<T> where T : Block {
        protected VoxelWorld world;
        protected Dictionary<Coord3, T> blocks;

        public BlockBehaviour<T> Awake(VoxelWorld world) {
            this.world = world;
            blocks = new Dictionary<Coord3, T>();
            return this;
        }

        public virtual void Add(Coord3 pos, T block) {
            blocks.Add(pos, block);
        }

        public virtual void Remove(Coord3 pos) {
            blocks.Remove(pos);
        }

        public virtual void UnloadChunk(Coord3 chunkPos) {
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