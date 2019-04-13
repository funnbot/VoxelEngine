using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;
using VoxelEngine.Utilities;

namespace VoxelEngine.Blocks {

    public class RotatedBlock : Block {
        public RotatedBlock() { }

        public Coord3 rotation;

        public void SetRotation(Coord3 rotation) {
            this.rotation = rotation.ModWrap(4);
        }

        public override void Serialize(System.IO.BinaryWriter writer) {
            rotation.Serialize(writer);
        }

        public override void Deserialize(System.IO.BinaryReader reader) {
            rotation.Deserialize(reader);
        }
    }

}