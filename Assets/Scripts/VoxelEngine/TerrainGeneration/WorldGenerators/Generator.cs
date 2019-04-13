using System.Collections.Generic;
using OpenSimplexNoise;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.TerrainGeneration {

    public abstract class Generator {
        private VoxelWorld world;
        private SimplexNoise noise;

        public Generator(VoxelWorld world, SimplexNoise noise) {
            this.world = world;
            this.noise = noise;
        }

        public void GenerateChunk(Chunk col) {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++) {
                var chunk = col.GetSection(i);
                GenerateSection(chunk);
                for (int x = 0; x < ChunkSection.Size; x++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        GenerateChunk(chunk, x, z);
                    }
                }
            }
        }

        protected virtual void GenerateChunk(ChunkSection chunk, int x, int z) {
            var stone = ResourceStore.Blocks["stone"];
            for (int y = 0; y < ChunkSection.Size; y++) {
                var pos = new Coord3(x, y, z);
                SetBlock(chunk, pos, stone);
            }
        }

        protected virtual void GenerateSection(ChunkSection chunk) { }

        protected Block SetBlock(ChunkSection chunk, Coord3 localPos, BlockData data, bool replace = false) {
            var b = chunk.blocks.GetBlock(localPos);
            if (replace || b == null) {
                Block outb;
                chunk.blocks.PlaceBlock(localPos, data, out outb, false);
                return outb;
            }
            return null;
        }

        protected int GetNoise(int x, int y, int z, float scale, int max) {
            return Mathf.FloorToInt((GetNoise(x * scale, y * scale, z * scale) + 1f) * (max / 2f));
        }
        protected int GetNoise(Coord3 pos, float scale, int max) =>
            GetNoise(pos.x, pos.y, pos.z, scale, max);
        protected int GetNoise(int x, int z, float scale, int max) =>
            GetNoise(x, 0, z, scale, max);
        private float GetNoise(float x, float y, float z) {
            return (float) noise.Evaluate(x, y, z);
        }

        protected BlockData GetBlockData(string id) =>
            ResourceStore.Blocks[id];

        protected BlockData[] GetBlocksOfType(BlockSpawnType type) {
            var blocks = new List<BlockData>(10);
            foreach (BlockData b in ResourceStore.Blocks) {
                if (b.spawnType == type) blocks.Add(b);
            }
            return blocks.ToArray();
        }

        protected bool GetChance(int x, int y, int z, float frequency, int density) {
            return GetNoise(x, y, z, frequency, 100) <= density;
        }

        protected void GenerateStructure(ChunkSection chunk, int x, int y, int z, string id) {
            var s = ResourceStore.Structures[id];

            for (int l = 0; l < s.height; l++) {
                for (int xi = 0; xi < s.size.x; xi++) {
                    for (int zi = 0; zi < s.size.y; zi++) {
                        var val = s[l][zi * s.size.y + xi];
                        var block = s.blocks[val];
                        var pos = new Coord3(x + xi, y + l, z + zi) - (Coord3) s.origin + Coord3.one;
                        SetBlock(chunk, pos, block, s.cutout);
                    }
                }
            }
        }

        protected void FillWithAir(ChunkSection chunk) {
            var air = ResourceStore.Blocks["air"];
            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        var pos = new Coord3(x, y, z);
                        SetBlock(chunk, pos, air);
                    }
                }
            }
        }
    }

}