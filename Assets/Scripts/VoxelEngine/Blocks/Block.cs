using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class Block {
        public BlockData data;
        public Coord3 position;
        public Coord3 rotation;

        public Block(BlockData data, Coord3 position = new Coord3(), Coord3 rotation = new Coord3()) {
            this.data = data;
            this.position = position;
            this.rotation = rotation;
        }

        public static class Face {
            public const int front = 0;
            public const int back = 1;
            public const int top = 2;
            public const int bottom = 3;
            public const int right = 4;
            public const int left = 5;
        }
    }

}