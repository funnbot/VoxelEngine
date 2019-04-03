using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;

namespace VoxelEngine.Internal {

    public class Block {
        public byte id;

        public virtual void Serialize(BinaryWriter writer) { }
        public virtual void Deserialize(BinaryReader reader) { }

        public virtual void OnPlace() { }
        public virtual void OnBreak() { }

        public virtual void OnLoad(Coord3 pos, BlockData data, ChunkSection chunk) { }
        public virtual void OnUnload() { }

        public virtual void OnNeighborUpdate() { }

        public static Block Convert(byte id, BlockDataType type) {
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

    public static class BlockFace {
        public const int front = 0;
        public const int back = 1;
        public const int top = 2;
        public const int bottom = 3;
        public const int right = 4;
        public const int left = 5;
    }

}