using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoxelEngine.ProceduralGeneration {

    public class FlatGenerator : Generator {
        int stoneHeight = 50;
        int dirtHeight = 4;
        float treeFrequency = 0.7f;
        int treeDensity = 15;
        float grassFrequency = 0.4f;
        int grassDensity = 24;
        float caveFrequency = 0.05f;
        int caveDensity = 20;

        BlockData air, stone, dirt, grass;
        BlockData[] ores;

        public FlatGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            air = ResourceStore.Blocks["air"];
            stone = ResourceStore.Blocks["stone"];
            dirt = ResourceStore.Blocks["dirt"];
            grass = ResourceStore.Blocks["grass"];

            ores = GetBlocksOfType(BlockType.Ore);
        }

        protected override void GenerateColumn(Chunk chunk, int x, int z) {
            var localPos = new Vector3Int(x, 0, z);
            void Set(BlockData block) {
                SetBlock(chunk, localPos, block);
            }
            for (localPos.y = -Chunk.Rollover; localPos.y < Chunk.Size + Chunk.Rollover; localPos.y++) {
                var pos = chunk.BlockToWorldPos(localPos);
                bool isCave = GetChance(pos.x, pos.y, pos.z, caveFrequency, caveDensity);

                if (pos.y <= stoneHeight && !isCave) {
                    for (int i = 0; i < ores.Length; i++) {
                        if (GetChance(pos.x, pos.y, pos.z, ores[i].spawnFrequency, ores[i].spawnDensity))
                            Set(ores[i]);
                        else Set(stone);
                    }
                } else if (pos.y < stoneHeight + dirtHeight && !isCave) Set(dirt);
                else if (pos.y == stoneHeight + dirtHeight && !isCave) Set(grass);
                else Set(air);
            }
        }
    }

}