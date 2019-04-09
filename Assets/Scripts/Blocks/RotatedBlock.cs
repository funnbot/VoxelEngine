using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.Blocks {

    public class RotatedBlock : Block {
        public RotatedBlock() { }

        public Coord3 rotation;

        public void SetRotation(Coord3 rotation) {
            this.rotation = rotation;
        }

        public override void Serialize(System.IO.BinaryWriter writer) {
            writer.Write(rotation);
        }

        public override void Deserialize(System.IO.BinaryReader reader) {
            rotation = reader.ReadCoord3();
        }
    }

}