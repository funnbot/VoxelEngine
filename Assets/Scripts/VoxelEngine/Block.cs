using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class Block {
        public BlockData data;
        public Coord3 rotation;

        public Coord3 position;
        public Chunk chunk;

        public Block(BlockData data, Coord3 rotation = new Coord3()) {
            this.data = data;
            this.rotation = rotation;
        }

        public Block(Block copy) {
            data = copy.data;
            rotation = copy.rotation;
            position = copy.position;
            chunk = copy.chunk;
        }

        public Block ConvertTo(System.Type type) {
            if (type == typeof(HelloBlock)) return new HelloBlock(this);

            throw new System.InvalidCastException($"Converting type \"{typeof(Block)}\" to type \"{type.Name}\" is not supported.");
        }
    }

    public static class BlockFace {
        public const int front = 0;
        public const int back = 1;
        public const int top = 2;
        public const int bottom = 3;
        public const int right = 4;
        public const int left = 5;
    }
}