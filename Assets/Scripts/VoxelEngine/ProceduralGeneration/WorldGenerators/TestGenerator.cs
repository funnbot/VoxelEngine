using OpenSimplexNoise;
using VoxelEngine.Data;

namespace VoxelEngine.ProceduralGeneration {

    public class TestGenerator : Generator {
        BlockData air, dirt, wood;

        public TestGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            air = GetBlockData("air");
            dirt = GetBlockData("grass_decal");
            wood = GetBlockData("wood");
        }

        protected override void GenerateColumn(ChunkSection chunk, int x, int z) {
            var localPos = new Coord3(x, 0, z);
            var pos = localPos.BlockToWorld(chunk.worldPosition);

            for (int y = 0; y < ChunkSection.Size; y++) {
                localPos.y = y;
                pos = localPos.BlockToWorld(chunk.worldPosition);

                SetBlock(chunk, localPos, air);
            }
        }
    }

}