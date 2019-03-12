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

        public ChunkColumnPool columnPool;
        //  public Dictionary<Coord3, Chunk> chunks;
        public Dictionary<Coord2, ChunkColumn> columns;
        public Dictionary<string, IBlockBehaviour> behaviours;

        public delegate void Tick();
        public event Tick OnTick;

        void Awake() {
            // chunks = new Dictionary<Coord3, Chunk>();
            columns = new Dictionary<Coord2, ChunkColumn>();
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

        public Block RegisterBlock(Block block, Coord3 position, Chunk chunk) {
            block.position = position;

            if (block.data.behaviour != "") {
                BlockBehaviour<Block> bb;
                if (GetBehaviour(block.data.behaviour, out bb)) {
                    bb.Add(position, block);
                }
            }

            if (block.data.meshType == BlockMeshType.Custom) {
                var go = Instantiate(block.data.prefab, position, Quaternion.Euler(block.rotation * 90), chunk.StandaloneBlocks);
                var sb = new StandaloneBlock(block);
                sb.gameObject = go;
                return sb;
            }
            if (block.data.dataType != "") {
                var t = System.Type.GetType(block.data.dataType, false, true);
                if (t != null) return (Block) System.Convert.ChangeType(block, t);
            }
            return block;
        }

        public void UnregisterBlock(Block block) {
            if (block == null) return;

            if (block.data.behaviour != "") {
                BlockBehaviour<Block> bb;
                if (GetBehaviour(block.data.behaviour, out bb)) {
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
                    var behaviour = (BlockBehaviour<Block>) bb.Value;
                    behaviour.UnloadChunk(c.position);
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
            behaviours = new Dictionary<string, IBlockBehaviour>();

            behaviours.Add(MovingBehaviour.name, new MovingBehaviour().Awake(this));
        }

        bool GetBehaviour(string name, out BlockBehaviour<Block> bb) {
            IBlockBehaviour genB;
            if (behaviours.TryGetValue(name, out genB)) {
                Debug.Log(genB.GetType().IsSubclassOf(typeof(BlockBehaviour<StandaloneBlock>)));
                var b = (BlockBehaviour<Block>)genB;
                bb = b;
                return true;
            }
            bb = null;
            return false;
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
        }
    }
}