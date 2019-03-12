using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {
    public class BlockBehaviour<T> : IBlockBehaviour where T : Block {
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

        public void UnloadChunk(Coord3 chunkPos) {
            foreach (var block in blocks) {
                var pos = world.BlockToChunkPos(block.Value.position);
                if (chunkPos == pos) Remove(block.Value.position);
            }
        }

        protected virtual void OnTick() {
            foreach (var block in blocks) {
                UpdateBlock(block.Value);
            }
        }

        protected virtual void UpdateBlock(T block) {

        }

        public virtual void AttachEvents() {
            world.OnTick += OnTick;
        }
    }

    public class IBlockBehaviour { 

    }
}