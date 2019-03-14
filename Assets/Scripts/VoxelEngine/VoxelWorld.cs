using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class VoxelWorld : MonoBehaviour {
        public static readonly int Height = 80;
        public static readonly int ChunkHeight = Height / Chunk.Size;

        public static VoxelWorld Active { get; private set; }

        public int renderAverage = 0;
        [HideInInspector]
        public int renderTime = 0;
        [HideInInspector]
        public int renderCount = 0;

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

        void Awake() {
            Active = this;
            columns = new Dictionary<Coord2, ChunkColumn>();
            seed = (int) (Random.value * 10000);
        }

        void Start() {
            generator = new ProceduralGenerator(this).Use(generatorType);

            LoadBehaviours();
            LoadSpawn();
        }

        void Update() {
            if (++tick == tickSpeed) {
                tick = 0;
                OnTick?.Invoke();
            }
            renderAverage = renderTime / renderCount;
        }

        public Block RegisterBlock(Block block, Coord3 position, Chunk chunk) {
            block.position = position;
            block.chunk = chunk;

            Block outBlock = block;
            if (block.data.meshType == BlockMeshType.Custom) {
                var go = Instantiate(block.data.prefab, position, Quaternion.Euler(block.rotation * 90), chunk.Blocks);
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
            return col;
        }

        public void DestroyColumn(Coord2 pos) {
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
            var chunkPos = pos.WorldToChunk();
            var chunk = GetChunk(chunkPos);
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
            var chunk = GetChunk(chunkPos);
            if (chunk == null) return;

            chunk.SetBlock(block, pos.WorldToBlock(chunk.worldPosition), update);
        }
        public void SetBlock(BlockData block, Coord3 pos, Coord3 rot, bool update = true) {
            var chunkPos = pos.WorldToChunk();
            var chunk = GetChunk(chunkPos);
            if (chunk == null) return;

            chunk.SetBlock(block, pos.WorldToBlock(chunk.worldPosition), rot, update);
        }
        public void SetBlock(BlockData block, RaycastHit hit, Coord3 rotation, bool adjacent = true) {
            var pos = Coord3.RaycastToBlock(hit, adjacent);
            SetBlock(block, pos, rotation, true);
        }

        void LoadBehaviours() {
            behaviours = new Dictionary<string, BlockBehaviour>();

            behaviours.Add(MovingBehaviour.name, new MovingBehaviour().Awake(this));
        }

        void LoadSpawn() {
            int size = spawnSize;
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

        void LoadSpawnThreaded() {
            int loadSize = spawnSize + 1;
            var cols = new ChunkColumn[loadSize * 2 + 1, loadSize * 2 + 1];

            for (int x = -loadSize; x <= loadSize; x++) {
                for (int z = -loadSize; z <= loadSize; z++) {
                    cols[x + loadSize, z + loadSize] = LoadColumn(new Coord2(x, z));
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