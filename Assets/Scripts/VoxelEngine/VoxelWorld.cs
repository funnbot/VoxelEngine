using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class VoxelWorld : MonoBehaviour {
        public const int Height = 80;
        public const int ChunkHeight = Height / Chunk.Size;

        public GameObject ChunkFab;
        public int seed = 1337;
        public int texturePixelResolution = 128;
        public GeneratorType generatorType = GeneratorType.Classic;
        public Generator generator;

        public int tickSpeed = 2;
        private int tick;

        public Dictionary<Vector3Int, Chunk> chunks;
        public Dictionary<string, BlockBehaviour<Block>> behaviours;

        public delegate void Tick();
        public event Tick OnTick;

        void Awake() {
            chunks = new Dictionary<Vector3Int, Chunk>();
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

        public Chunk LoadChunk(Vector3Int pos) {
            if (chunks.ContainsKey(pos)) return chunks[pos];
            var chunkObject = Instantiate(ChunkFab).transform;
            chunkObject.parent = transform;
            chunkObject.localPosition = pos * Chunk.Size;
            var chunk = chunkObject.GetComponent<Chunk>();
            chunk.Init(this, pos);
            chunks.Add(pos, chunk);
            return chunk;
        }
        public void LoadChunks(Vector2Int col) {
            for (int i = 0; i < ChunkHeight; i++) {
                var pos = new Vector3Int(col.x, i, col.y);
                LoadChunk(pos);
            }
        }

        public void DestroyChunk(Vector3Int pos) {
            Chunk chunk;
            if (chunks.TryGetValue(pos, out chunk)) {
                chunk.Destroy();
                Destroy(chunk.gameObject);
                chunks.Remove(pos);
            }
        }
        public void DestroyChunks(Vector2Int col) {
            for (int i = 0; i < ChunkHeight; i++) {
                var pos = new Vector3Int(col.x, i, col.y);
                DestroyChunk(pos);
            }
        }

        public void BuildChunks(Vector2Int col) {
            var pos = new Vector3Int(col.x, 0, col.y);
            var chunk = GetChunk(pos);
            generator.GenerateChunks(pos);
        }

        public void RenderChunks(Vector2Int col) {
            for (int i = 0; i < ChunkHeight; i++) {
                var pos = new Vector3Int(col.x, i, col.y);
                chunks[pos].Render();
            }
        }

        public void UpdateChunks(Chunk chunk, Vector3Int blockPos) {
            chunk.update = true;
            if (blockPos.MaxElem() != Chunk.Size - 1 && blockPos.MinElem() != 0) return;
            foreach (var offset in Chunk.DirOffsets) {
                // Find the direction the neighbor chunk is in
                var pos = blockPos + offset;
                if (pos.MaxElem() >= Chunk.Size || pos.MinElem() < 0) {
                    var cPos = chunk.position + offset;
                    var c = GetChunk(cPos);
                    if (c == null) continue;
                    c.update = true;
                }
            }
        }

        public Chunk GetChunk(Vector3Int chunkPos) {
            Chunk chunk;
            if (chunks.TryGetValue(chunkPos, out chunk))
                return chunk;
            else return null;
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
            chunk.SetBlock(pos, block);
            if (updateChunk) UpdateChunks(chunk, pos);
        }
        public void SetBlock(RaycastHit hit, Block block, bool adjacent = true) {
            var pos = RaycastToBlockPos(hit.point, hit.normal, adjacent);
            SetBlock(pos, block, true);
        }

        public Vector3Int BlockToChunkPos(Vector3Int pos) =>
            pos.Div(Chunk.Size);

        public Vector3Int RaycastToBlockPos(Vector3 point, Vector3 normal, bool adjacent) =>
            Vector3Int.FloorToInt(point + Vector3.one * 0.5f + (adjacent ? -Vector3.Max(normal, Vector3.zero) : -Vector3.Min(normal, Vector3.zero)));

        void LoadBehaviours() {
            behaviours = new Dictionary<string, BlockBehaviour<Block>>();

            behaviours.Add("block", new BlockBehaviour<Block>().Awake(this));
        }

        void LoadSpawn() {
            for (int x = -8; x <= 8; x++) {
                for (int z = -8; z <= 8; z++) {
                    var pos = new Vector2Int(x, z);
                    LoadChunks(pos);
                    BuildChunks(pos);
                }
            }
            // Render the 3x3 spawn area
            for (int x = -7; x <= 7; x++) {
                for (int z = -7; z <= 7; z++) {
                    var pos = new Vector2Int(x, z);
                    RenderChunks(pos);
                }
            }
        }
    }
}