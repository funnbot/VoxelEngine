using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;
using VoxelEngine.Utilities;
using System.IO;

namespace VoxelEngine.Blocks {

    public class RotatedBlock : CustomBlock {
        public RotatedBlock(byte id) : base(id) { }

        public Coord3 rotation;

        public void SetRotation(Coord3 rotation) {
            this.rotation = rotation.ModWrap(4);
        }

        public override void Serialize(BinaryWriter writer) {
            rotation.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader) {
            rotation.Deserialize(reader);
        }
    }

}