﻿using OpenSimplexNoise;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.TerrainGeneration {

    public class ClassicGenerator : Generator {
        // Base water level
        const int waterLevel = 30;
        const int beachLevel = 33;
        const int plainsLevel = 50;
        const int mountainsLevel = 70;

        const int terrainMaxHeight = 80;
        const float terrainNoiseScale = 0.012f;

        const int minorTerrainMaxHeight = 6;
        const float minorTerrainNoiseScale = 0.101f;

        const int sandMaxHeight = 4;
        const float sandNoiseScale = 0.13f;

        const int stoneDepth = 5;
        const int stoneMaxHeight = 15;

        const int mountainMaxHeight = 20;
        const float mountainNoiseScale = 0.013f;

        const float caveNoiseScale1 = 0.026f;
        const float caveNoiseScale2 = 0.13f;
        const float caveNoiseScale3 = 0.078f;

        const float treeFrequency = 0.8f;
        const int treeDensity = 12;

        const float grassFrequency = 0.4f;
        const int grassDensity = 26;

        const float ruinsFrequency = 0.8f;
        const int ruinsDensity = 10;

        const float ironOreFrequency = 0.4f;
        const int ironOreDensity = 26;

        // Noise
        // Base terrain height for deciding biome
        int terrainHeight;
        int sandHeight;
        int mountainHeight;

        int grassNoise;
        int treeNoise;

        BlockData stone, dirt, grass, grass_decal, water, sand, iron_ore;

        public ClassicGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            stone = GetBlockData("stone");
            dirt = GetBlockData("dirt");
            grass = GetBlockData("grass");
            grass_decal = GetBlockData("grass_decal");
            water = GetBlockData("water");
            sand = GetBlockData("sand");
            iron_ore = GetBlockData("iron_ore");
        }

        protected override void GenerateNoise(Chunk col, int x, int z) {
            terrainHeight = GetNoiseFloored(x, z, terrainNoiseScale, terrainMaxHeight);
            terrainHeight += GetNoiseFloored(x, z, minorTerrainNoiseScale, minorTerrainMaxHeight);

            if (terrainHeight <= waterLevel) {
                sandHeight = GetNoiseFloored(x, z, sandNoiseScale, sandMaxHeight);
            }
        }
        // agile#9002
        protected override void GenerateChunk(ChunkSection chunk, int x, int z) {
            var worldPos = new Coord3(x, 0, z).BlockToWorld(chunk.worldPosition);

            for (int y = 0; y < ChunkSection.Size; y++) {
                var localPos = new Coord3(x, y, z);
                worldPos = localPos.BlockToWorld(chunk.worldPosition);

                if (terrainHeight <= waterLevel) {
                    bool IsNotCave = NotCave(worldPos);
                    if (worldPos.y < stoneMaxHeight && IsNotCave) SetBlock(chunk, localPos, stone);
                    else if (worldPos.y <= terrainHeight - sandHeight && IsNotCave) SetBlock(chunk, localPos, sand);
                    else if (worldPos.y <= waterLevel) SetBlock(chunk, localPos, water);
                } else if (terrainHeight <= beachLevel) {
                    if (worldPos.y <= terrainHeight && NotCave(worldPos)) SetBlock(chunk, localPos, sand);
                } else {
                    int hgt = beachLevel + (terrainHeight - beachLevel) / (2);
                    bool IsNotCave = NotCave(worldPos);
                    if (worldPos.y <= hgt - stoneDepth && IsNotCave) {
                        if (GetNoiseFloored(x, y, z, ironOreFrequency, 100) < ironOreDensity) SetBlock(chunk, localPos, iron_ore);
                        else SetBlock(chunk, localPos, stone);
                    }
                    else if (worldPos.y < hgt && IsNotCave) SetBlock(chunk, localPos, dirt);
                    else if (worldPos.y == hgt && IsNotCave) SetBlock(chunk, localPos, grass);
                    else if (worldPos.y == hgt + 1 && IsNotCave) {
                        if (GetNoiseFloored(worldPos, grassFrequency, 100) < grassDensity)
                            SetBlock(chunk, localPos, grass_decal);
                        else if (GetNoiseFloored(worldPos, treeFrequency, 100) < treeDensity)
                            GenerateStructure(chunk, x, y, z, "tree");
                        else if (GetNoiseFloored(worldPos, ruinsFrequency, 100) < ruinsDensity)
                            GenerateStructure(chunk, x, y, z, "ruins");
                    }
                }
            }
        }

        private bool NotCave(Coord3 wpos) {
            int caveNoise = GetNoiseFloored(wpos.y, wpos.x, wpos.z, caveNoiseScale1, 10 / 3);
            caveNoise += GetNoiseFloored(wpos.x, wpos.y, wpos.z, caveNoiseScale2, 10 / 3);
            caveNoise += GetNoiseFloored(wpos.x, wpos.z, wpos.y, caveNoiseScale3, 10 / 3);
            return caveNoise < 5;
        }

    }

}