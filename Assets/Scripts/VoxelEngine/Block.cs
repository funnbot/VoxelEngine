using MessagePack;
using VoxelEngine.Data;
using VoxelEngine.Blocks;

namespace VoxelEngine {

    [System.Serializable, MessagePackObject]
    public class Block {
        [Key(0)]
        public string dataName;
        [Key(1)]
        public Coord3 rotation;
        [Key(2)]
        public Coord3 position;

        [System.NonSerialized, IgnoreMember]
        public BlockData data;
        [System.NonSerialized, IgnoreMember]
        public Chunk chunk;

        public Block(BlockData data, Coord3 rotation = new Coord3()) {
            this.data = data;
            this.rotation = rotation;
            dataName = data.blockName;
        }

        public Block(Block copy) {
            data = copy.data;
            dataName = copy.dataName;
            rotation = copy.rotation;
            position = copy.position;
            chunk = copy.chunk;
        }

        [SerializationConstructor]
        public Block(string dataName, Coord3 rotation, Coord3 position) {
            this.dataName = dataName;
            this.rotation = rotation;
            this.position = position;
        }

        public Block ConvertTo(System.Type type) {
            if (type == typeof(HelloBlock)) return new HelloBlock(this);

            throw new System.InvalidCastException($"Converting type \"{typeof(Block)}\" to type \"{type.Name}\" is not supported.");
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