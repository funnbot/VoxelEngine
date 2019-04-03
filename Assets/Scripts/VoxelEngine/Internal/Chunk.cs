using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Internal;
using VoxelEngine.Serialization;
using BinaryWriter = System.IO.BinaryWriter;
using BinaryReader = System.IO.BinaryReader;

namespace VoxelEngine.Internal {

    public class Chunk : MonoBehaviour {
        public GameObject chunkFab;

        public Coord2 position { get; private set; }

        public bool built { get; private set; }
        public bool generated { get; private set; }
        public bool rendered { get; private set; }
        public bool isDirty { get; set; }

        private VoxelWorld world;
        private ChunkSection[] chunks;

        private SerialChunk serialChunk;

        public void Create(VoxelWorld world) {
            this.world = world;

            chunks = new ChunkSection[VoxelWorld.ChunkHeight];
            for (int i = 0; i < chunks.Length; i++) {
                chunks[i] = Instantiate(chunkFab).GetComponent<ChunkSection>();
                chunks[i].Create(this, world);
            }

            serialChunk = new SerialChunk();
            serialChunk.blocks = new Block[VoxelWorld.ChunkHeight][][][];
        }

        public void Setup(Coord2 position) {
            this.position = position;

            built = false;
            generated = false;
            rendered = false;
            isDirty = false;

            transform.parent = world.transform;
            transform.localPosition = (Coord3) position * ChunkSection.Size;
            name = "Chunk " + position;

            for (int i = 0; i < chunks.Length; i++) {
                var pos = new Coord3(position.x, i, position.y);
                chunks[i].Setup(pos);
            }
        }

        public void CleanUp() {
            foreach (var chunk in chunks)
                chunk.CleanUp();
        }

        public void Serialize(BinaryWriter writer) {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].blocks.Serialize(writer);
        }

        public void Deserialize(BinaryReader reader) {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].blocks.Deserialize(reader);
        }

        public void LoadBlocks() {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].blocks.LoadAll();
        }

        public void UnloadBlocks() {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].blocks.UnloadAll();
        }

        public ChunkSection GetSection(int y) {
            if (InRange(y)) return chunks[y];
            else return null;
        }

        public void Build() {
            if (Serializer.IsChunkSaved(world.saveName, position)) {
                Serializer.LoadChunk(world.saveName, position, this);
                LoadBlocks();
            } else world.generator.GenerateChunk(this);
            built = true;
        }

        public async Task BuildTask() {
            if (Serializer.IsChunkSaved(world.saveName, position)) {
                await Task.Run(() => Serializer.LoadChunk(world.saveName, position, this));
                LoadBlocks();
            } else await Task.Run(() => world.generator.GenerateChunk(this));
            built = true;
        }

        public void GenerateMesh() {
            foreach (var chunk in chunks)
                chunk.GenerateMesh();
            generated = true;
        }

        public async Task GenerateMeshTask() {
            await Task.Run(() => {
                foreach (var chunk in chunks)
                    chunk.GenerateMesh();
            });
            generated = true;
        }

        public void ApplyMesh() {
            if (!generated) return;

            foreach (var chunk in chunks)
                chunk.ApplyMesh();
            rendered = true;
        }

        public void QueueRender() {
            foreach (var chunk in chunks)
                chunk.QueueUpdate();
            generated = true;
            rendered = true;
        }

        public void Save() {
            if (!isDirty || !built) return;
            Serializer.SaveChunk(world.saveName, position, this);
            UnloadBlocks();
            isDirty = false;
        }

        public async Task SaveTask() {
            if (!isDirty || !built) return;
            await Task.Run(() => Serializer.SaveChunk(world.saveName, position, this));
            UnloadBlocks();
            isDirty = false;
        }

        private bool InRange(int y) => y >= 0 && y < chunks.Length;
    }

}