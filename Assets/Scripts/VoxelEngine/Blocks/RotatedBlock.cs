using Ceras;
using UnityEngine;
using VoxelEngine.Data;

namespace VoxelEngine.Blocks {

    public class RotatedBlock : Block {
        public Coord3 rotation;

        public RotatedBlock(Block copy) : base(copy) { }
        public RotatedBlock(BlockData data, Coord3 rotation) {
            this.data = data;
            this.id = data.blockID;
            this.rotation = rotation;
        }
        public RotatedBlock() { }
    }

}