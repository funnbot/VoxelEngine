//using MessagePack;
using Ceras;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;

namespace VoxelEngine {

    //[MessagePackObject]
    public class Block {
        public string id;

        [Exclude]
        public Coord3 position;

        [Exclude]
        public BlockData data;

        [Exclude]
        public ChunkSection chunk;

        public Block(BlockData data) {
            this.data = data;
            id = data.blockID;
        }

        public Block(Block copy) {
            data = copy.data;
            id = copy.id;
        }

        public Block() { }

        public Block ConvertTo(string type) {
            if (type == "VirusBlock") return new VirusBlock(this);
            if (type == "MinerBlock") return new MinerBlock(this);
            if (type == "PipeBlock") return new PipeBlock(this);

            throw new System.InvalidCastException($"Converting type \"{typeof(Block)}\" to type \"{type}\" is not supported.");
        }

        protected void UpdateNeighbors() {
            foreach (var dir in Coord3.Directions) {
                var block = chunk.GetBlock(position.WorldToBlock(chunk.worldPosition) + dir);
                block.OnNeighborUpdate(this);
            }
        }

        public virtual void OnBreak() { }
        public virtual void OnPlace() { }

        public virtual void OnLoad() { }
        public virtual void OnUnload() { }

        public virtual void OnNeighborUpdate(Block block) { }
        
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