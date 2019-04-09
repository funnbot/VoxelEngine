using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Internal;
using VoxelEngine.Serialization;
using BinaryWriter = System.IO.BinaryWriter;
using BinaryReader = System.IO.BinaryReader;

namespace VoxelEngine.Internal {

    public class Chunk : MonoBehaviour {
        /// Inspector Reference
        public GameObject chunkFab;

        /// Position of this chunk
        public Coord2 position { get; private set; }

        public bool IsBuilt { get; private set; }
        public bool IsGenerated { get; private set; }
        public bool IsRendered { get; private set; }
        public bool IsDirty { get; set; }

        private VoxelWorld world;
        private ChunkSection[] chunks;

        /// Create this chunk and put it in the pool
        public void Create(VoxelWorld world) {
            this.world = world;

            chunks = new ChunkSection[VoxelWorld.ChunkHeight];
            for (int i = 0; i < chunks.Length; i++) {
                chunks[i] = Instantiate(chunkFab).GetComponent<ChunkSection>();
                chunks[i].Create(this, world);
            }
        }

        /// Setup this chunk pulled from the pool
        public void Setup(Coord2 position) {
            this.position = position;

            IsBuilt = false;
            IsGenerated = false;
            IsRendered = false;
            IsDirty = false;

            transform.parent = world.transform;
            transform.localPosition = (Coord3) position * ChunkSection.Size;
            name = "Chunk " + position;

            for (int i = 0; i < chunks.Length; i++) {
                var pos = new Coord3(position.x, i, position.y);
                chunks[i].Setup(pos);
            }
        }

        /// Cleanup all chunks and release to the pool
        public void CleanUp() {
            foreach (var chunk in chunks)
                chunk.CleanUp();
        }

        /// Get a section within this chunk
        public ChunkSection GetSection(int y) {
            if (InRange(y)) return chunks[y];
            else return null;
        }

        /// Rerendering the neighboring chunks
        public void UpdateNeighbors() {
            foreach (var dir in Coord2.Directions) {
                var chunk = world.chunks.GetChunk(position + dir);
                if (chunk != null && chunk.IsRendered) {
                    chunk.Render();
                }
            }
        }

        /// Load blocks into the chunk
        public void Build() {
            if (Serializer.IsChunkSaved(world.saveName, position)) {
                Serializer.LoadChunk(world.saveName, position, this);
                LoadBlocks();
            } else world.generator.GenerateChunk(this);
            IsBuilt = true;
        }

        /// Generate the mesh from the blocks
        public void GenerateMesh() {
            if (!IsBuilt) return;
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].GenerateMesh();
            IsGenerated = true;
        }

        /// Apply the generated mesh to the renderers
        public void ApplyMesh() {
            if (!IsGenerated) return;
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].ApplyMesh();
            IsRendered = true;
        }

        /// Chunks will only render when allowed to
        public void Render() {
            if (!IsBuilt) return;
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].Render();
            IsGenerated = IsRendered = true;
        }

        public void Save() {
            if (IsDirty && IsBuilt)
                Serializer.SaveChunk(world.saveName, position, this);
            UnloadBlocks();
            IsDirty = false;
        }

        /// Serialize all chunk sections
        public void Serialize(BinaryWriter writer) {
            if (!IsBuilt) return;
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].blocks.Serialize(writer);
        }

        /// Deserialize all chunk sections
        public void Deserialize(BinaryReader reader) {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].blocks.Deserialize(reader);
        }

        /// Load the blocks after deserializing
        public void LoadBlocks() {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].blocks.LoadAll();
        }

        /// Unload the blocks after serializing
        public void UnloadBlocks() {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].blocks.UnloadAll();
        }

        private bool InRange(int y) => y >= 0 && y < chunks.Length;
    }

}