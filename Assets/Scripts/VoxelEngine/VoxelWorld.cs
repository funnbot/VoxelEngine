using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class VoxelWorld : MonoBehaviour {
        public static readonly int Height = 80;
        public static readonly int ChunkHeight = Height / Chunk.Size;

        public GameObject ColumnFab;
        public int seed = 1347;
        public int texturePixelResolution = 128;
        public GeneratorType generatorType = GeneratorType.Classic;
        public Generator generator;
        public float scale = 1.0f;

        public int tickSpeed = 10;
        private int tick;

        public PrefabPool columnPool;
        public Dictionary<Vector3Int, Chunk> chunks;
        public Dictionary<Vector2Int, ChunkColumn> columns;
        public Dictionary<string, BlockBehaviour<Block>> behaviours;

        public delegate void Tick();
        public event Tick OnTick;

        void Awake() {
            chunks = new Dictionary<Vector3Int, Chunk>();
            columns = new Dictionary<Vector2Int, ChunkColumn>();
        }

        void Start() {
            generator = new ProceduralGenerator(this).Use(generatorType);

            LoadBehaviours();
            LoadSpawn();
        }

        void Update() {
            if (++tick == tickSpeed) {
                tick = 0;
                if (OnTick != null) OnTick();
            }
        }

        public ChunkColumn LoadColumn(Vector2Int pos) {
            if (columns.ContainsKey(pos)) return columns[pos];

            var chunk = Instantiate(ColumnFab).GetComponent<ChunkColumn>();
            chunk.Init(this, pos);
            columns.Add(pos, chunk);
            return chunk;
        }

        public void DestroyColumn(Vector2Int pos) {
            ChunkColumn col = columns[pos];

            col.Destroy();
            Destroy(col.gameObject);
            columns.Remove(pos);
        }

        public Chunk GetChunk(Vector3Int pos) {
            var col = GetColumn(pos.ToVec2());
            if (col == null) return null;
            return col.GetChunk(pos.y);
        }

        public ChunkColumn GetColumn(Vector2Int pos) {
            ChunkColumn col;
            if (columns.TryGetValue(pos, out col)) {
                return col;
            } else return null;
        }

        public Block GetBlock(Vector3Int pos) {
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

        public void SetBlock(Vector3Int pos, Block block, bool updateChunk = false) {
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

        public Vector3Int BlockToChunkPos(Vector3Int pos) =>
            pos.Div(Chunk.Size);

        public Vector3Int RaycastToBlockPos(Vector3 point, Vector3 normal, bool adjacent) =>
            Vector3Int.FloorToInt(point + Vector3.one * 0.5f + (adjacent ? -Vector3.Max(normal, Vector3.zero) : Vector3.Min(normal, Vector3.zero)));

        void LoadBehaviours() {
            behaviours = new Dictionary<string, BlockBehaviour<Block>>();

            behaviours.Add("block", new BlockBehaviour<Block>().Awake(this));
        }

        void LoadSpawn() {
            int size = 1;
            for (int x = -size - 1; x <= size + 1; x++) {
                for (int z = -size - 1; z <= size + 1; z++) {
                    var pos = new Vector2Int(x, z);
                    var col = LoadColumn(pos);
                    col.Build();
                }
            }
            // Render the 3x3 spawn area
            for (int x = -size; x <= size; x++) {
                for (int z = -size; z <= size; z++) {
                    var pos = new Vector2Int(x, z);
                    var col = GetColumn(pos);
                    col.Render();
                }
            }
        }
    }
}