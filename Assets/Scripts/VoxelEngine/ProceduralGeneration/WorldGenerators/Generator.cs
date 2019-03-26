using System.Collections.Generic;
using OpenSimplexNoise;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;

namespace VoxelEngine.ProceduralGeneration {

    public abstract class Generator {
        private VoxelWorld world;
        private SimplexNoise noise;

        public Generator(VoxelWorld world, SimplexNoise noise) {
            this.world = world;
            this.noise = noise;
        }

        public void GenerateColumn(Chunk col) {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++) {
                var chunk = col.GetSection(i);
                GenerateChunk(chunk);
                for (int x = 0; x < ChunkSection.Size; x++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        GenerateColumn(chunk, x, z);
                    }
                }
            }
        }

        protected virtual void GenerateColumn(ChunkSection chunk, int x, int z) {
            var stone = ResourceStore.Blocks["stone"];
            for (int y = 0; y < ChunkSection.Size; y++) {
                var pos = new Coord3(x, y, z);
                SetBlock(chunk, pos, stone);
            }
        }

        protected virtual void GenerateChunk(ChunkSection chunk) { }

        protected void SetBlock(ChunkSection chunk, Coord3 localPos, Block block, bool replace = false) {
            var b = chunk.GetBlock(localPos);
            if (replace || b == null || b.data.meshType == BlockMeshType.Air) {
                chunk.SetBlock(block, localPos, false);
            }
        }
        protected void SetBlock(ChunkSection chunk, Coord3 localPos, BlockData blockData, Coord3 rotation = new Coord3(), bool replace = false) {
            var b = chunk.GetBlock(localPos);
            if (replace || b == null || b.data.meshType == BlockMeshType.Air) {
                chunk.SetBlock(blockData, localPos, rotation, false);
            }
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

        protected BlockData[] GetBlocksOfType(BlockType type) {
            var blocks = new List<BlockData>(10);
            foreach (KeyValuePair<string, BlockData> b in ResourceStore.Blocks) {
                if (b.Value.blockType == type) blocks.Add(b.Value);
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
                        SetBlock(chunk, pos, block, Coord3.zero, s.cutout);
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