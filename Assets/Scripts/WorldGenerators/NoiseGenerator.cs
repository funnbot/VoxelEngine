using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.ProceduralGeneration {

    public class NoiseGenerator : Generator {
        int baseHeight = 10;
        int noiseHeight = 30;
        float noiseFreq = 0.02f;

        bool chanceNoise = false;

        float freq = 0.1f;
        int dens = 30;

        bool noise3d = true;

        BlockData air, dirt;

        public NoiseGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            air = GetBlockData("air");
            dirt = GetBlockData("stone");
        }

        protected override void GenerateColumn(Chunk chunk, int x, int z) {
            var localPos = new Vector3Int(x, 0, z);
            var pos = chunk.BlockToWorldPos(localPos);

            var height = baseHeight + GetNoise(pos.x, 0, pos.z, noiseFreq, noiseHeight);

            for (int y = 0; y < Chunk.Size; y++) {
                localPos.y = y;
                pos = chunk.BlockToWorldPos(localPos);

                if (chanceNoise) {
                    if (noise3d || pos.y == 0) {
                        var chance = GetChance(pos.x, pos.y, pos.z, freq, dens);
                        if (chance) SetBlock(chunk, localPos, dirt);
                        else SetBlock(chunk, localPos, air);
                    }
                } else {
                    if (pos.y <= height) SetBlock(chunk, localPos, dirt);
                    else SetBlock(chunk, localPos, air);
                }
            }
        }
    }

}