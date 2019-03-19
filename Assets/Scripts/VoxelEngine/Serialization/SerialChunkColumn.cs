using MessagePack;
using VoxelEngine.Blocks;

namespace VoxelEngine.Serialization {

    [MessagePackObject]
    public class SerialChunkColumn {
        [Key(0)]
        public Block[][][][] blocks;
    }

}