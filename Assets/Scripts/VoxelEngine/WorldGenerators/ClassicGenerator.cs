﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.ProceduralGeneration {

    public class ClassicGenerator : Generator {
        int stoneBaseHeight = 24;
        float stoneBaseNoise = 0.02f;
        int stoneBaseNoiseHeight = 4;
        int stoneMountainHeight = 48;
        float stoneMountainFrequency = 0.04f;
        int stoneMinHeight = 34;
        int dirtBaseHeight = 1;
        float dirtNoise = 0.04f;
        int dirtNoiseHeight = 3;
        float caveFrequency = 0.05f;
        int caveSize = 20;
        float treeFrequency = 0.7f;
        int treeDensity = 16;
        float grassFrequency = 0.4f;
        int grassDensity = 26;

        BlockData air, stone, dirt, grass, grass_decal;

        public ClassicGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            air = ResourceStore.Blocks["air"];
            stone = ResourceStore.Blocks["stone"];
            dirt = ResourceStore.Blocks["dirt"];
            grass = ResourceStore.Blocks["grass"];
            grass_decal = ResourceStore.Blocks["grass_decal"];
        }

        protected override void GenerateColumn(Chunk chunk, int x, int z) {
            var worldPos = chunk.BlockToWorldPos(new Vector3Int(x, 0, z));

            int stoneHeight = stoneBaseHeight;
            stoneHeight += GetNoise(worldPos.x, worldPos.z, stoneMountainFrequency, stoneMountainHeight);
            if (stoneHeight < stoneMinHeight) stoneHeight = stoneMinHeight;
            stoneHeight += GetNoise(worldPos.x, worldPos.z, stoneBaseNoise, stoneBaseNoiseHeight);
            int dirtHeight = stoneHeight + dirtBaseHeight;
            dirtHeight += GetNoise(worldPos.x, worldPos.z, dirtNoise, dirtNoiseHeight);

            for (int y = -Chunk.Rollover; y < Chunk.Size + Chunk.Rollover; y++) {
                var localPos = new Vector3Int(x, y, z);
                worldPos = chunk.BlockToWorldPos(localPos);

                int caveChance = GetNoise(worldPos, caveFrequency, 100);
                if (worldPos.y <= stoneHeight && caveSize < caveChance) {
                    SetBlock(chunk, localPos, stone);
                } else if (worldPos.y <= dirtHeight && caveSize < caveChance) {
                    if (worldPos.y == dirtHeight) SetBlock(chunk, localPos, grass);
                    else SetBlock(chunk, localPos, dirt);

                    if (worldPos.y == dirtHeight && GetNoise(worldPos.x, worldPos.z, treeFrequency, 100) < treeDensity)
                        GenerateStructure(chunk, localPos.x, localPos.y + 1, localPos.z, "tree");
                    if (worldPos.y == dirtHeight && GetNoise(worldPos.x, worldPos.z, grassFrequency, 100) < grassDensity)
                        SetBlock(chunk, new Vector3Int(localPos.x, localPos.y + 1, localPos.z), grass_decal);
                } else {
                    SetBlock(chunk, localPos, air);
                }
            }
        }
    }

}