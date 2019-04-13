using OpenSimplexNoise;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.TerrainGeneration {

    public class TestGenerator : Generator {
        BlockData proto, proto_obj;

        public TestGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            proto = GetBlockData("proto");
            proto_obj = GetBlockData("proto_obj");
        }

        protected override void GenerateChunk(ChunkSection chunk, int x, int z) {
            if (chunk.position != Coord3.zero) return;

            for (int y = 0; y < 4; y++) {

                if (x >= 0 && x <= 3 && z >= 0 && z <= 3) {
                    var pos = new Coord3(8 * x, 8 * y, 8 * z);
                    var block = SetBlock(chunk, pos, proto);
                    chunk.blocks.GetCustomBlock<RotatedBlock>(block, b => b.SetRotation(new Coord3(x, y, z)));

                    var block_obj = SetBlock(chunk, new Coord3(8 * x, 8 * y + 2, 8 * z), proto_obj);
                    chunk.blocks.GetCustomBlock<StandaloneBlock>(block_obj, b => b.gameObject.transform.localEulerAngles = new Vector3(x * 90, y * 90, z * 90));
                }

            }
        }
    }

}