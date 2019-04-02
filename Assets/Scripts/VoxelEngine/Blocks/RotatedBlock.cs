using Ceras;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.Blocks {

    public class RotatedBlock : Block {
        public RotatedBlock() { }

        [Include]
        public Coord3 rotation;

        public void SetRotation(Coord3 rotation) {
            this.rotation = rotation;
        }
    }

}