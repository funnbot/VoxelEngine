using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class Block {
        public BlockData data;
        public Coord3 position;
        public Coord2 rotation;

        public Block(BlockData data, Coord3 position = new Coord3(), Coord2 rotation = new Coord2()) {
            this.data = data;
            this.position = position;
            this.rotation = rotation;
        }
    }
    
}