﻿using OpenSimplexNoise;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.TerrainGeneration {

    public class FlatGenerator : Generator {
        int stoneHeight = 20;
        int dirtHeight = 4;
        float treeFrequency = 0.5f;
        int treeDensity = 11;
        float grassFrequency = 0.6f;
        int grassDensity = 25;

        float caveFrequency = 0.01f;
        int caveDensity = 7;

        BlockData stone, dirt, grass, grass_decal;
        BlockData[] ores;

        public FlatGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            stone = ResourceStore.Blocks["stone"];
            dirt = ResourceStore.Blocks["dirt"];
            grass = ResourceStore.Blocks["grass"];
            grass_decal = ResourceStore.Blocks["grass_decal"];

            ores = GetBlocksOfType(BlockSpawnType.Ore);
        }

        protected override void GenerateChunk(ChunkSection chunk, int x, int z) {
            var localPos = new Coord3(x, 0, z);
            void Set(BlockData block) {
                SetBlock(chunk, localPos, block);
            }
            for (localPos.y = 0; localPos.y < ChunkSection.Size; localPos.y++) {
                var pos = localPos.BlockToWorld(chunk.worldPosition);
                bool isCave = GetChance(pos.x, pos.y, pos.z, caveFrequency, caveDensity) ||
                    GetChance(pos.y, pos.z, pos.x, caveFrequency + 0.02f, caveDensity + 4) ||
                    GetChance(-pos.x, -pos.y, -pos.z, caveFrequency - 0.02f, caveDensity - 4);

                if (pos.y <= stoneHeight && !isCave) {
                    for (int i = 0; i < ores.Length; i++) {
                        if (GetChance(pos.x, pos.y, pos.z, ores[i].spawnFrequency, ores[i].spawnDensity))
                            Set(ores[i]);
                        else Set(stone);
                    }
                } else if (pos.y < stoneHeight + dirtHeight && !isCave) Set(dirt);
                else if (pos.y == stoneHeight + dirtHeight && !isCave) {
                    if (GetChance(pos.x, 0, pos.z, treeFrequency, treeDensity))
                        GenerateStructure(chunk, localPos.x, localPos.y + 1, localPos.z, "tree");
                    else if (GetChance(pos.x, 0, pos.z, grassFrequency, grassDensity))
                        SetBlock(chunk, new Coord3(localPos.x, localPos.y + 1, localPos.z), grass_decal);
                    Set(grass);
                }
            }
        }
    }

}