using MessagePack;
using VoxelEngine.Data;
using VoxelEngine.Blocks;

namespace VoxelEngine {

    [System.Serializable, MessagePackObject]
    public class Block {
        [Key(0)]
        public string id;
        [Key(1)]
        public Coord3 rotation;

        [System.NonSerialized, IgnoreMember]
        public Coord3 position;
        [System.NonSerialized, IgnoreMember]
        public BlockData data;
        [System.NonSerialized, IgnoreMember]
        public ChunkSection chunk;

        public Block(BlockData data, Coord3 rotation = new Coord3()) {
            this.data = data;
            this.rotation = rotation;
            id = data.blockID;
        }

        public Block(Block copy) {
            data = copy.data;
            id = copy.id;
            rotation = copy.rotation;
            position = copy.position;
            chunk = copy.chunk;
        }

        [SerializationConstructor]
        public Block(string id, Coord3 rotation) {
            this.id = id;
            this.rotation = rotation;
        }

        public Block ConvertTo(string type) {
            if (type == "VirusBlock") return new VirusBlock(this);

            throw new System.InvalidCastException($"Converting type \"{typeof(Block)}\" to type \"{type}\" is not supported.");
        }
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