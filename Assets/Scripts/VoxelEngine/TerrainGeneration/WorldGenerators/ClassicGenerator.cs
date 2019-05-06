using OpenSimplexNoise;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.TerrainGeneration {

    public class ClassicGenerator : Generator {
        // Base water level
        const int waterLevel = 30;
        const int beachLevel = 33;
        const int plainsLevel = 50;

        const int terrainMaxHeight = 80;
        const float terrainNoiseScale = 0.012f;

        const int minorTerrainMaxHeight = 4;
        const float minorTerrainNoiseScale = 0.101f;

        const int sandMaxHeight = 4;
        const float sandNoiseScale = 0.15f;

        const int stoneMinHeight = 10;
        const float stoneMinNoiseScale = 0.28f;

        const float caveNoiseScale1 = 0.026f;
        const float caveNoiseScale2 = 0.13f;
        const float caveNoiseScale3 = 0.078f;

        const float treeFrequency = 0.7f;
        const int treeDensity = 16;

        const float grassFrequency = 0.4f;
        const int grassDensity = 26;

        const float ruinsFrequency = 0.8f;
        const int ruinsDensity = 10;

        // Noise
        // Base terrain height for deciding biome
        int terrainHeight;
        int sandHeight;

        int grassNoise;
        int treeNoise;

        BlockData stone, dirt, grass, grass_decal, water, sand;

        public ClassicGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            stone = GetBlockData("stone");
            dirt = GetBlockData("dirt");
            grass = GetBlockData("grass");
            grass_decal = GetBlockData("grass_decal");
            water = GetBlockData("water");
            sand = GetBlockData("sand");
        }

        protected override void GenerateNoise(Chunk col, int x, int z) {
            terrainHeight = GetNoiseFloored(x, z, terrainNoiseScale, terrainMaxHeight);
            terrainHeight += GetNoiseFloored(x, z, minorTerrainNoiseScale, minorTerrainMaxHeight);

            if (terrainHeight <= waterLevel) {
                sandHeight = GetNoiseFloored(x, z, sandNoiseScale, sandMaxHeight);
            }
        }

        protected override void GenerateChunk(ChunkSection chunk, int x, int z) {
            var worldPos = new Coord3(x, 0, z).BlockToWorld(chunk.worldPosition);

            for (int y = 0; y < ChunkSection.Size; y++) {
                var localPos = new Coord3(x, y, z);
                worldPos = localPos.BlockToWorld(chunk.worldPosition);

                if (terrainHeight <= waterLevel) {
                    if (worldPos.y <= terrainHeight - sandHeight && NotCave(worldPos)) SetBlock(chunk, localPos, sand);
                    else if (worldPos.y <= waterLevel) SetBlock(chunk, localPos, water);
                } else if (terrainHeight <= beachLevel) {
                    if (worldPos.y <= terrainHeight && NotCave(worldPos)) SetBlock(chunk, localPos, sand);
                } else {
                    if (worldPos.y <= terrainHeight - stoneMinHeight && NotCave(worldPos)) SetBlock(chunk, localPos, stone);
                    else if (worldPos.y < terrainHeight && NotCave(worldPos)) SetBlock(chunk, localPos, dirt);
                    else if (worldPos.y == terrainHeight && NotCave(worldPos)) SetBlock(chunk, localPos, grass);
                    else if (worldPos.y == terrainHeight + 1 && NotCave(worldPos)) {
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