using OpenSimplexNoise;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.TerrainGeneration {

    public class ClassicGenerator : Generator {
        float biomeNoiseScale = 0.02f;
        int waterBiome = 0;
        int plainsBiome = 1;
        int mountainBiome = 2;

        // water
        int waterSandMinHeight;
        int waterSandMaxHeight;
        float waterSandNoise;

        int waterHeight = 50;

        int waterIslandNoise;
        int waterIslandSize;

        // plains
        int plainsStoneHeight;

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

        BlockData stone, dirt, grass, grass_decal, water, sand;

        public ClassicGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            stone = GetBlockData("stone");
            dirt = GetBlockData("dirt");
            grass = GetBlockData("grass");
            grass_decal = GetBlockData("grass_decal");
            water = GetBlockData("water");
            sand = GetBlockData("sand");
        }

        protected void GeneratesChunk(ChunkSection chunk, int x, int z) {
            var worldPos = new Coord3(x, 0, z).BlockToWorld(chunk.worldPosition);

            // Get current biome
            int biomeIndex = GetNoise(worldPos.x, worldPos.z, biomeNoiseScale, 2);

            int stoneHeight = stoneBaseHeight;
            stoneHeight += GetNoise(worldPos.x, worldPos.z, stoneMountainFrequency, stoneMountainHeight);

            if (stoneHeight < stoneMinHeight) stoneHeight = stoneMinHeight;
            stoneHeight += GetNoise(worldPos.x, worldPos.z, stoneBaseNoise, stoneBaseNoiseHeight);

            int dirtHeight = stoneHeight + dirtBaseHeight;
            dirtHeight += GetNoise(worldPos.x, worldPos.z, dirtNoise, dirtNoiseHeight);

            for (int y = 0; y < ChunkSection.Size; y++) {
                var localPos = new Coord3(x, y, z);
                worldPos = localPos.BlockToWorld(chunk.worldPosition);

                int caveChance = GetNoise(worldPos, caveFrequency, 100);
                if (worldPos.y <= stoneHeight && caveSize < caveChance) {
                    SetBlock(chunk, localPos, stone);
                } else if (worldPos.y <= dirtHeight && caveSize < caveChance) {
                    if (worldPos.y == dirtHeight) SetBlock(chunk, localPos, grass);
                    else SetBlock(chunk, localPos, dirt);

                    if (worldPos.y == dirtHeight && GetNoise(worldPos.x, worldPos.z, treeFrequency, 100) < treeDensity)
                        GenerateStructure(chunk, localPos.x, localPos.y + 1, localPos.z, "tree");
                    else if (worldPos.y == dirtHeight && GetNoise(worldPos.x, worldPos.z, grassFrequency, 100) < grassDensity)
                        SetBlock(chunk, new Coord3(localPos.x, localPos.y + 1, localPos.z), grass_decal);
                }
            }
        }

        int biomeIndex;
        int stoneHeight;

        protected override void GenerateColumn(Chunk col) {
            int x = col.position.x, z = col.position.y;
            biomeIndex = GetNoise(x, z, biomeNoiseScale, 3);
            
        }

        protected override void GenerateChunk(ChunkSection chunk, int x, int z) {
            var worldPos = new Coord3(x, 0, z).BlockToWorld(chunk.worldPosition);



            for (int y = 0; y < ChunkSection.Size; y++) {
                var localPos = new Coord3(x, y, z);
                worldPos = localPos.BlockToWorld(chunk.worldPosition);

                if (biomeIndex == waterBiome) {
                    SetBlock(chunk, localPos, water);
                } else if (biomeIndex == plainsBiome) {
                    SetBlock(chunk, localPos, dirt);
                } else if (biomeIndex == mountainBiome) {
                    SetBlock(chunk, localPos, stone);
                }
            }
        }

    }

}