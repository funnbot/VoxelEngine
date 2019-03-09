using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.ProceduralGeneration {

    public class TestGenerator : Generator {
        BlockData air, dirt, wood;

        public TestGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            air = GetBlockData("air");
            dirt = GetBlockData("grass_decal");
            wood = GetBlockData("wood");
        }

        protected override void GenerateColumn(Chunk chunk, int x, int z) {
            var localPos = new Coord3(x, 0, z);
            var pos = chunk.BlockToWorldPos(localPos);

            for (int y = 0; y < Chunk.Size; y++) {
                localPos.y = y;
                pos = chunk.BlockToWorldPos(localPos);

                SetBlock(chunk, localPos, air);

                if (localPos == new Coord3(0, 0, 0)) {
                    Coord3 rot = new Coord3(y % 5, 0, 0);
                    var block = new Block(wood, pos, rot);
                    SetBlock(chunk, localPos, block);
                }
            }
        }
    }

}