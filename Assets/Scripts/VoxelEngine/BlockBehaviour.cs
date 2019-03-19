using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {
    public class BlockBehaviour {
        protected VoxelWorld world;
        protected Dictionary<Coord3, Block> blocks;

        public BlockBehaviour Awake(VoxelWorld world) {
            this.world = world;
            blocks = new Dictionary<Coord3, Block>();
            AttachEvents();
            return this;
        }

        public virtual void Add(Coord3 pos, Block block) {
            blocks.Add(pos, block);
        }

        public virtual void Remove(Coord3 pos) {
            blocks.Remove(pos);
        }

        public void UnloadChunk(Coord3 chunkPos) {
            foreach (var block in blocks) {
                var pos = block.Value.position.WorldToChunk();
                if (chunkPos == pos) Remove(block.Value.position);
            }
        }

        protected virtual void OnTick() {
            foreach (var block in blocks) {
                UpdateBlock(block.Value);
            }
        }

        protected virtual void UpdateBlock(Block block) { }

        protected virtual void AttachEvents() {
            world.OnTick += OnTick;
        }
    }
}