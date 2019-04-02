using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;

namespace VoxelEngine.Internal {

    public class Block {
        public int id;

        public virtual void Serialize(BinaryWriter writer) {
            writer.Write((byte) id);
        }

        public virtual void Deserialize(BinaryReader reader) {
            id = reader.ReadByte();
        }

        public virtual void OnPlace() { }
        public virtual void OnBreak() { }

        public virtual void OnLoad(Coord3 pos, BlockData data, ChunkSection chunk) { }
        public virtual void OnUnload() { }

        public virtual void OnNeighborUpdate() { }

        public static Block Convert(int id, BlockDataType type) {
            if (type == BlockDataType.RotatedBlock) return new RotatedBlock { id = id };
            if (type == BlockDataType.StandaloneBlock) return new StandaloneBlock { id = id };
            if (type == BlockDataType.PipeBlock) return new PipeBlock { id = id };
            if (type == BlockDataType.VirusBlock) return new VirusBlock { id = id };
            if (type == BlockDataType.MinerBlock) return new MinerBlock { id = id };

            throw new System.InvalidCastException("Cannot cast block type");
        }
    }

    public enum BlockDataType {
        None = 0,
        RotatedBlock,
        StandaloneBlock,

        VirusBlock,
        PipeBlock,
        MinerBlock,
    }

}