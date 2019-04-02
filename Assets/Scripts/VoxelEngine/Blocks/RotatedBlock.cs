using Ceras;
using UnityEngine;
using VoxelEngine.Data;

namespace VoxelEngine.Blocks {

    public class RotatedBlock : Block {
        public RotatedBlock(Block copy) : base(copy) {
            RotatedBlock block = copy as RotatedBlock;
            if (block != null) rotation = block.rotation;
        }
        public RotatedBlock() { }

        [Include]
        public Coord3 rotation;

        public RotatedBlock(BlockData data, Coord3 rotation) {
            this.data = data;
            this.id = data.blockId;
            this.byteId = data.byteId;
            this.rotation = rotation;
        }
    }

}