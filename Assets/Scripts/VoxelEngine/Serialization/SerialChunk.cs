using MessagePack;
using VoxelEngine.Blocks;

namespace VoxelEngine.Serialization {

    [MessagePackObject]
    public class SerialChunk {
        [Key(0)]
        public Block[][][][] blocks;
    }

}