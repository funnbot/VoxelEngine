using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;

namespace VoxelEngine.Internal {

    public class Block {
        public byte id;

        public Block(byte id) {
            this.id = id;
        }
    }

}