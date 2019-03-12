﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {
    using Watch = System.Diagnostics.Stopwatch;

    public class VoxelWorld : MonoBehaviour {
        public static readonly int Height = 80;
        public static readonly int ChunkHeight = Height / Chunk.Size;

        public static VoxelWorld Active { get; private set; }

        public GameObject ColumnFab;
        public int seed = 1347;
        public int texturePixelResolution = 128;
        public GeneratorType generatorType = GeneratorType.Classic;
        public Generator generator;
        public int spawnSize;

        public int tickSpeed = 10;
        private int tick;

        public ChunkColumnPool columnPool;
        public Dictionary<Coord2, ChunkColumn> columns;
        public Dictionary<string, BlockBehaviour> behaviours;

        public delegate void Tick();
        public event Tick OnTick;

        public delegate void SpawnLoaded();
        public event SpawnLoaded OnSpawnLoad;

        public delegate void ColumnLoaded(Coord2 pos);
        public event ColumnLoaded OnColumnLoad;

        public delegate void ColumnUnloaded(Coord2 pos);
        public event ColumnUnloaded OnColumnUnload;

        void Awake() {
            Active = this;
            columns = new Dictionary<Coord2, ChunkColumn>();
            seed = (int)(Random.value * 10000);
        }

        void Start() {
            generator = new ProceduralGenerator(this).Use(generatorType);

            LoadBehaviours();
            var sw = Watch.StartNew();
            LoadSpawn();
            sw.Stop();
            Debug.Log("Spawn load time: " + sw.ElapsedMilliseconds);
        }

        void Update() {
            if (++tick == tickSpeed) {
                tick = 0;
                OnTick?.Invoke();
            }
        }

        public Block RegisterBlock(Block block, Coord3 position, Chunk chunk) {
            block.position = position;

            Block outBlock = block;
            if (block.data.meshType == BlockMeshType.Custom) {
                var go = Instantiate(block.data.prefab, position, Quaternion.Euler(block.rotation * 90), chunk.StandaloneBlocks);
                go.name = block.data.name + " " + block.position;
                var sb = new StandaloneBlock(block);
                sb.gameObject = go;
                outBlock = sb;
            }
            if (block.data.dataType.Length > 0) {
                var t = System.Type.GetType(block.data.dataType, false, true);
                if (t != null) outBlock = block.ConvertTo(t);
            }

            if (block.data.behaviour != "") {
                BlockBehaviour bb;
                if (behaviours.TryGetValue(block.data.behaviour, out bb)) {
                    bb.Add(position, outBlock);
                }
            }

            return outBlock;
        }

        public void UnregisterBlock(Block block) {
            if (block == null) return;

            if (block.data.behaviour != "") {
                BlockBehaviour bb;
                if (behaviours.TryGetValue(block.data.behaviour, out bb)) {
                    bb.Remove(block.position);
                }
            }

            if (block.data.meshType == BlockMeshType.Custom && block is StandaloneBlock) {
                var sb = (StandaloneBlock) block;
                if (sb.gameObject != null) Destroy(sb.gameObject, 0.1f);
            }
        }

        public ChunkColumn LoadColumn(Coord2 pos) {
            if (columns.ContainsKey(pos)) return columns[pos];

            var col = columnPool.GetObject();
            col.Init(pos);
            columns.Add(pos, col);
            OnColumnLoad?.Invoke(pos);
            return col;
        }

        public void DestroyColumn(Coord2 pos) {
            OnColumnUnload?.Invoke(pos);
            ChunkColumn col = columns[pos];

            foreach (var bb in behaviours) {
                foreach (var c in col.chunks) {
                    bb.Value.UnloadChunk(c.position);
                }
            }

            columnPool.ReleaseObject(col);
            columns.Remove(pos);
        }

        public Chunk GetChunk(Coord3 pos) {
            var col = GetColumn((Coord2) pos);
            if (col == null) return null;
            return col.GetChunk(pos.y);
        }

        public ChunkColumn GetColumn(Coord2 pos) {
            ChunkColumn col;
            if (columns.TryGetValue(pos, out col)) {
                return col;
            } else return null;
        }

        public Block GetBlock(Coord3 pos) {
            var chunkPos = BlockToChunkPos(pos);
            var chunk = GetChunk(chunkPos);
            if (chunk == null) return null;

            var blockPos = chunk.WorldToBlockPos(pos);
            return chunk.GetBlock(blockPos);
        }
        public Block GetBlock(RaycastHit hit, bool adjacent = false) {
            var pos = RaycastToBlockPos(hit.point, hit.normal, adjacent);
            return GetBlock(pos);
        }

        public void SetBlock(Coord3 pos, Block block, bool updateChunk = false) {
            var chunkPos = BlockToChunkPos(pos);
            var chunk = GetChunk(chunkPos);
            if (chunk == null) return;

            pos = chunk.WorldToBlockPos(pos);
            chunk.SetBlock(pos, block, updateChunk);
        }
        public void SetBlock(RaycastHit hit, Block block, bool adjacent = true) {
            var pos = RaycastToBlockPos(hit.point, hit.normal, adjacent);
            SetBlock(pos, block, true);
        }

        public Coord3 BlockToChunkPos(Coord3 pos) =>
            pos.FloorDiv(Chunk.Size);

        public Coord3 RaycastToBlockPos(Vector3 point, Vector3 normal, bool adjacent) =>
            Coord3.FloorToInt(point + Vector3.one * 0.5f + (adjacent ? -Vector3.Max(normal, Vector3.zero) : Vector3.Min(normal, Vector3.zero)));

        void LoadBehaviours() {
            behaviours = new Dictionary<string, BlockBehaviour>();

            behaviours.Add(MovingBehaviour.name, new MovingBehaviour().Awake(this));
        }

        void LoadSpawn() {
            int size = 1;
            for (int x = -size - 1; x <= size + 1; x++) {
                for (int z = -size - 1; z <= size + 1; z++) {
                    var pos = new Coord2(x, z);
                    var col = LoadColumn(pos);
                    col.Build();
                }
            }

            // Render the 3x3 spawn area
            for (int x = -size; x <= size; x++) {
                for (int z = -size; z <= size; z++) {
                    var pos = new Coord2(x, z);
                    var col = GetColumn(pos);
                    col.Render();
                }
            }

            OnSpawnLoad?.Invoke();
        }

        IEnumerator LoadSpawnThreaded() {
            int loadSize = spawnSize + 1;
            var tasks = new List<Task>(loadSize * loadSize);
            var cols = new ChunkColumn[loadSize, loadSize];

            for (int x = -loadSize; x <= loadSize; x++) {
                for (int z = -loadSize; z <= loadSize; z++) {
                    var pos = new Coord2(x, z);
                    var col = LoadColumn(pos);
                    var task = Task.Run(() => col.Build());
                    cols[x, z] = col;
                    tasks.Add(task);
                }
            }

            while (!tasks.TrueForAll(t => t.IsCompleted)) 
                yield return null;

            for (int x = -spawnSize; x <= spawnSize; x++) {
                for (int z = -spawnSize; z < spawnSize; z++) {
                    cols[x, z].Render();
                }
            }
        }
    }
}