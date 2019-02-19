using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine.ProceduralGeneration {

    public class ProceduralGenerator {
        Dictionary<GeneratorType, Generator> generators;
        SimplexNoise noise;

        public ProceduralGenerator(VoxelWorld world) {
            noise = new SimplexNoise(world.seed);
            generators = new Dictionary<GeneratorType, Generator>();

            generators.Add(GeneratorType.Classic, new ClassicGenerator(world, noise));
            generators.Add(GeneratorType.Flat, new FlatGenerator(world, noise));
        }

        public Generator Use(GeneratorType type) =>
            generators[type];
    }

    public enum GeneratorType {
        Classic,
        Flat
    }
}

public static class TerrainGen {

    static SimplexNoise Noise = new SimplexNoise();

    public static void GenerateChunks(Chunk chunk) {
        FillWithAir(chunk);
        for (int x = 0; x < Chunk.Size; x++) {
            for (int z = 0; z < Chunk.Size; z++) {
                chunk = GenerateChunkCol(chunk, x, z);
            }
        }
    }

    static Chunk GenerateChunkCol(Chunk chunk, int x, int z) {
        var stone = ResourceStore.Blocks["stone"];
        var grass = ResourceStore.Blocks["grass"];
        var grass_decal = ResourceStore.Blocks["grass_decal"];
        Vector3 pos = new Vector3Int(x, 0, z) + chunk.transform.position;

        int stoneHeight = 10;
        int height = GetNoise((int) pos.x, 0, (int) pos.z, 0.04f, 10);
        for (int y = 0; y < VoxelWorld.Height; y++) {
            if (y < stoneHeight + height) chunk.SetBlock(new Vector3Int(x, y, z), new Block(stone));
            else if (y < stoneHeight + height + 4) chunk.SetBlock(new Vector3Int(x, y, z), new Block(grass));
            else if (Random.value > 0.99f && y < stoneHeight + height + 5) SetBlock(chunk, x, y, z, grass_decal);
            else if (Random.value > 0.999f && y < stoneHeight + height + 5) GenerateTree(ref chunk, x, y, z);

        }
        return chunk;
    }

    static void FillWithAir(Chunk chunk) {
        var air = ResourceStore.Blocks["air"];
        for (int x = 0; x < Chunk.Size; x++) {
            for (int y = 0; y < VoxelWorld.Height; y++) {
                for (int z = 0; z < Chunk.Size; z++) {
                    SetBlock(chunk, x, y, z, air);
                }
            }
        }
    }

    static void SetBlock(Chunk chunk, int x, int y, int z, BlockData block, bool replace = false) {
        var b = chunk.GetBlock(new Vector3Int(x, y, z));
        if (replace || b == null || b.data.meshType == BlockMeshType.Air) {
            chunk.SetBlock(new Vector3Int(x, y, z), new Block(block));
        }
    }

    static int GetNoise(int x, int y, int z, float scale, int max) {
        return Mathf.FloorToInt((GetNoise(x * scale, y * scale, z * scale) + 1f) * (max / 2f));
    }

    static float GetNoise(float x, float y, float z) {
        return (float) Noise.Evaluate(x, y, z);
    }

    static bool GetChance(int x, int y, int z, float frequency, int density) {
        return GetNoise(x, y, z, frequency, 100) < density;
    }

    static void GenerateTree(ref Chunk chunk, int x, int y, int z) {
        GenerateStructure(chunk, x, y, z, "tree");
    }

    static void GenerateStructure(Chunk chunk, int x, int y, int z, string id) {
        var s = ResourceStore.Structures[id];

        for (int l = 0; l < s.height; l++) {
            for (int xi = 0; xi < s.size.x; xi++) {
                for (int zi = 0; zi < s.size.y; zi++) {
                    var val = s[l][zi * s.size.y + xi];
                    var block = s.blocks[val];
                    var pos = new Vector3Int(x + xi, y + l, z + zi) - (Vector3Int) s.origin + Vector3Int.one;
                    SetBlock(chunk, pos.x, pos.y, pos.z, block, s.cutout);
                }
            }
        }
    }
}