using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.Blocks {

    public class CustomBlock : Block {
        public CustomBlock(byte id) : base(id) { }
        
        protected BlockData data;
        protected ChunkSection chunk;
        protected Coord3 position;

        public void Setup(BlockData data, ChunkSection chunk, Coord3 position) {
            this.position = position;
            this.chunk = chunk;
            this.data = data;
        }

        public virtual void Serialize(BinaryWriter writer) { }
        public virtual void Deserialize(BinaryReader reader) { }

        public virtual void OnPlace() { }
        public virtual void OnBreak() { }

        public virtual void OnLoad() { }
        public virtual void OnUnload() { }

        public virtual void OnNeighborUpdate() { }

        public static Block Convert(byte id, BlockDataType type) {
            if (type == BlockDataType.RotatedBlock) return new RotatedBlock(id);
            if (type == BlockDataType.StandaloneBlock) return new StandaloneBlock(id);
            if (type == BlockDataType.PipeBlock) return new PipeBlock(id);
            if (type == BlockDataType.VirusBlock) return new VirusBlock(id);
            if (type == BlockDataType.MinerBlock) return new MinerBlock(id);

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