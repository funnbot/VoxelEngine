using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Serialization;

namespace VoxelEngine {

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

        public void Init(Coord2 position) {
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
                chunks[i].Init(pos);
            }
        }

        public void CleanUp() {
            foreach (var chunk in chunks)
                chunk.CleanUp();
        }

        public SerialChunk Serialize() {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].Serialize(serialChunk, i);
            return serialChunk;
        }

        public void Deserialize() {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].Deserialize(serialChunk, i);
        }

        public ChunkSection GetSection(int y) {
            if (InRange(y)) return chunks[y];
            else return null;
        }

        public void Build() {
            built = true;

            if (Serializer.LoadChunk(world.saveName, position, ref serialChunk))
                this.Deserialize();
            else world.generator.GenerateColumn(this);
            isDirty = false;
        }

        public void GenerateMesh() {
            generated = true;
            foreach (var chunk in chunks)
                chunk.GenerateMesh();
        }

        public void ApplyMesh() {
            rendered = true;
            foreach (var chunk in chunks)
                chunk.ApplyMesh();
        }

        public void QueueRender() {
            generated = true;
            rendered = true;
            foreach (var chunk in chunks)
                chunk.QueueUpdate();
        }

        public void Save() {
            if (!built || !isDirty) return;

            Serializer.SaveChunk(world.saveName, position, this.Serialize());
            isDirty = false;
        }

        private bool InRange(int y) => y >= 0 && y < chunks.Length;
    }

}