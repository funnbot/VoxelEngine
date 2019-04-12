using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;
using VoxelEngine.Utilities;

namespace VoxelEngine.Blocks {

    public class RotatedBlock : Block {
        public RotatedBlock() { }

        public Coord3 rotation;

        public void SetRotation(Coord3 rotation) {
            // rotation = (Coord3)(Quaternion.Euler(rotation * 90).eulerAngles) / 90;
            this.rotation = rotation.ModWrap(4);
            Debug.Log(this.rotation);
        }

        public override void Serialize(System.IO.BinaryWriter writer) {
            writer.Write(rotation);
        }

        public override void Deserialize(System.IO.BinaryReader reader) {
            rotation = reader.ReadCoord3();
        }
    }

}