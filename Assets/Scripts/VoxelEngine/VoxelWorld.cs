using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Pooling;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class VoxelWorld : MonoBehaviour {
        public static readonly int Height = 80;
        public static readonly int ChunkHeight = Height / ChunkSection.Size;
        public static readonly int MaxRendersPerTick = 5 * ChunkHeight;

        public string saveName;
        public int seed;
        public GeneratorType generatorType = GeneratorType.Classic;
        public Generator generator;
        public int spawnSize;

        public int chunkRenders;

        public int tickSpeed;
        private int tick;

        public ChunkPool columnPool;
        public ChunkManager chunks;

        public delegate void Tick();
        public event Tick OnTick;

        public delegate void SpawnLoaded();
        public event SpawnLoaded OnSpawnLoad;

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
                chunks.UpdateChunks();
            }
        }

        public void InitializeBlock(ref Block block, Coord3 position, ChunkSection chunk) {
            if (block.data.dataType.Length > 0) {
                block = block.ConvertTo(block.data.dataType);
            }

            RegisterBlock(ref block, position, chunk);

            if (block.data.dataType.Length > 0) {
                IPlaceHandler placeHandler = block as IPlaceHandler;
                if (placeHandler != null) {
                    placeHandler.OnPlace();
                }
            }
        }

        public void DestroyBlock(Block block) {
            if (block.data.dataType.Length > 0) {
                IBreakHandler breakHandler = block as IBreakHandler;
                if (breakHandler != null) {
                    breakHandler.OnBreak();
                }
            }

            UnregisterBlock(block);
        }

        public void RegisterBlock(ref Block block, Coord3 position, ChunkSection chunk) {
            block.position = position;
            block.chunk = chunk;

            if (block.data.blockType == BlockType.Custom) {
                var go = Instantiate(block.data.prefab, position, Quaternion.Euler(block.rotation * 90), chunk.Blocks);
                go.name = block.data.blockID + " " + block.position;

                StandaloneBlock standalone = block as StandaloneBlock;
                if (standalone == null) standalone = new StandaloneBlock(block);

                standalone.gameObject = go;
                block = standalone;
            }

            if (block.data.dataType.Length > 0) {
                IUpdateable inter = block as IUpdateable;
                if (inter != null) {
                    OnTick += inter.OnTick;
                }
            }
        }

        public void UnregisterBlock(Block block) {
            if (block.data.dataType.Length > 0) {
                IUpdateable inter = block as IUpdateable;
                if (inter != null) {
                    OnTick -= inter.OnTick;
                }
            }

            if (block.data.blockType == BlockType.Custom) {
                StandaloneBlock standalone = block as StandaloneBlock;
                if (standalone != null && standalone.gameObject != null) {
                    Destroy(standalone.gameObject, 0.1f);
                }
            }
        }

        public void SpawnEntity(Block block, Coord3 pos, ChunkSection chunk) {
            var go = Instantiate(block.data.prefab, pos.BlockToWorld(chunk.worldPosition), Quaternion.Euler(block.rotation * 90), transform);
            go.name = block.data.blockID + " (Entity)";
        }

        public Block GetBlock(Coord3 pos) {
            var chunkPos = pos.WorldToChunk();
            var chunk = chunks.GetSection(chunkPos);
            if (chunk == null) return null;

            var blockPos = pos.WorldToBlock(chunk.worldPosition);
            return chunk.GetBlock(blockPos);
        }
        public Block GetBlock(RaycastHit hit, bool adjacent = false) {
            var pos = Coord3.RaycastToBlock(hit, adjacent);
            return GetBlock(pos);
        }

        public void SetBlock(Block block, Coord3 pos, bool update = true) {
            var chunkPos = pos.WorldToChunk();
            var chunk = chunks.GetSection(chunkPos);
            if (chunk == null) return;

            chunk.SetBlock(block, pos.WorldToBlock(chunk.worldPosition), update);
        }
        public void SetBlock(BlockData block, Coord3 pos, Coord3 rot, bool update = true) {
            var chunkPos = pos.WorldToChunk();
            var chunk = chunks.GetSection(chunkPos);
            if (chunk == null) return;

            chunk.SetBlock(block, pos.WorldToBlock(chunk.worldPosition), rot, update);
        }
        public void SetBlock(BlockData block, RaycastHit hit, Coord3 rotation, bool adjacent = true) {
            var pos = Coord3.RaycastToBlock(hit, adjacent);
            SetBlock(block, pos, rotation, true);
        }

        void LoadSpawn() {
            int size = spawnSize;
            for (int x = -size - 1; x <= size + 1; x++) {
                for (int z = -size - 1; z <= size + 1; z++) {
                    var pos = new Coord2(x, z);
                    var col = chunks.LoadChunk(pos);
                    col.Build();
                }
            }

            // Render the 3x3 spawn area
            for (int x = -size; x <= size; x++) {
                for (int z = -size; z <= size; z++) {
                    var pos = new Coord2(x, z);
                    var col = chunks.GetChunk(pos);
                    col.QueueRender();
                }
            }

            OnSpawnLoad?.Invoke();
        }

        void LoadSpawnThreaded() {
            int loadSize = spawnSize + 1;
            var cols = new Chunk[loadSize * 2 + 1, loadSize * 2 + 1];

            for (int x = -loadSize; x <= loadSize; x++) {
                for (int z = -loadSize; z <= loadSize; z++) {
                    cols[x + loadSize, z + loadSize] = chunks.LoadChunk(new Coord2(x, z));
                }
            }

            Parallel.For(-loadSize, loadSize + 1, x => {
                for (int z = -loadSize; z <= loadSize; z++) {
                    cols[x + loadSize, z + loadSize].Build();
                    cols[x + loadSize, z + loadSize].GenerateMesh();
                }
            });

            for (int x = -spawnSize; x <= spawnSize; x++) {
                for (int z = -spawnSize; z <= spawnSize; z++) {
                    cols[x + loadSize + 1, z + loadSize + 1].ApplyMesh();
                }
            }

            OnSpawnLoad?.Invoke();
        }
    }
}