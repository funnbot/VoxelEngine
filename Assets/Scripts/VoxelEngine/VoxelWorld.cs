using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Internal;
using VoxelEngine.Pooling;
using VoxelEngine.TerrainGeneration;

namespace VoxelEngine {

    public class VoxelWorld : MonoBehaviour {
        /// The amount of sections in chunk columns
        public static readonly int ChunkHeight = 5;
        /// How many chunk rerenders to do per tick
        public static readonly int MaxRendersPerTick = 5 * ChunkHeight;

        public string saveName;
        public int seed;
        public GeneratorType generatorType = GeneratorType.Classic;
        public Generator generator;
        private int spawnSize = 20;

        public Material WaterBlockMaterial;
        private int waterUpdateIndex;
        private int waterUpdateTick;

        public int chunkRenders;

        public int tickSpeed;
        private int tick;

        public ChunkPool columnPool;
        public ChunkManager chunks;

        public delegate void Tick();
        public event Tick OnTick;

        public delegate void SpawnLoaded();
        public event SpawnLoaded OnSpawnLoad;

        [ContextMenu("Start")]
        void Start() {
            generator = new ProceduralGenerator(this).Use(generatorType);
            chunks = new ChunkManager(columnPool);

            LoadSpawn();
        }

        void Update() {
            if (++tick == tickSpeed) {
                tick = 0;
                chunkRenders = 0;
                OnTick?.Invoke();
            }
            if (++waterUpdateTick == tickSpeed ) {
                waterUpdateTick = 0;
                UpdateWaterMaterial();
            }
        }

        public Block GetBlock(Coord3 pos) {
            var chunkPos = pos.WorldToChunk();
            var chunk = chunks.GetSection(chunkPos);
            if (chunk == null) return null;

            var blockPos = pos.WorldToBlock(chunk.worldPosition);
            return chunk.blocks.GetBlock(blockPos);
        }
        public Block GetBlock(RaycastHit hit, bool adjacent = false) {
            var pos = Coord3.RaycastToBlock(hit, adjacent);
            return GetBlock(pos);
        }

        public ChunkSection PlaceBlock(Coord3 worldPos, BlockData data, bool updateChunk = true) {
            var sectionPos = worldPos.WorldToChunk();
            var section = chunks.GetSection(sectionPos);
            if (section == null) return null;

            section.blocks.PlaceBlock(worldPos.WorldToBlock(section.worldPosition), data, updateChunk);
            return section;
        }
        public ChunkSection PlaceBlock(Coord3 worldPos, BlockData data, out Block block, bool updateChunk = true) {
            var sectionPos = worldPos.WorldToChunk();
            var section = chunks.GetSection(sectionPos);
            if (section == null) {
                block = null;
                return null;
            }

            section.blocks.PlaceBlock(worldPos.WorldToBlock(section.worldPosition), data, out block, updateChunk);
            return section;
        }
        public ChunkSection PlaceBlock(RaycastHit hit, BlockData data, bool adjacent = false) {
            var pos = Coord3.RaycastToBlock(hit, adjacent);
            return PlaceBlock(pos, data, true);
        }
        public ChunkSection PlaceBlock(RaycastHit hit, BlockData data, out Block block, bool adjacent = false) {
            var pos = Coord3.RaycastToBlock(hit, adjacent);
            return PlaceBlock(pos, data, out block, true);
        }

        private int waterSize = 32;
        void UpdateWaterMaterial() {
            float offset = 1f / waterSize;
            WaterBlockMaterial.mainTextureOffset = 
                new Vector2(0, waterUpdateIndex * offset);
            if (++waterUpdateIndex >= waterSize)
                waterUpdateIndex = 0;
        }

        void LoadSpawn() {
            int loadSize = spawnSize + 1;
            for (int x = -loadSize; x <= loadSize; x++) {
                for (int z = -loadSize; z <= loadSize; z++) {
                    var pos = new Coord2(x, z);
                    var chunk = chunks.CreateChunk(pos);
                }
            }

            for (int x = -spawnSize; x <= spawnSize; x++) {
                for (int z = -spawnSize; z <= spawnSize; z++) {
                    var pos = new Coord2(x, z);
                    var chunk = chunks.GetChunk(pos);
                    chunk.Build();
                }
            }

            for (int x = -spawnSize; x <= spawnSize; x++) {
                for (int z = -spawnSize; z <= spawnSize; z++) {
                    var pos = new Coord2(x, z);
                    var chunk = chunks.GetChunk(pos);
                    chunk.GenerateMesh();
                    chunk.ApplyMesh();
                }
            }

            OnSpawnLoad?.Invoke();
        }
    }
}