using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Serialization;
using VoxelEngine.Internal;

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

        public void Serialize() {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].Serialize(serialChunk, i);
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
            if (Serializer.IsChunkSaved(world.saveName, position)) {
                Serializer.LoadChunk(world.saveName, position, ref serialChunk);
                this.Deserialize();
            } else world.generator.GenerateColumn(this);
        }

        public async Task BuildThreaded() {
            built = true;
            if (Serializer.IsChunkSaved(world.saveName, position)) {
                await Task.Run(() => Serializer.LoadChunk(world.saveName, position, ref serialChunk));
                this.Deserialize();
            } else await Task.Run(() => world.generator.GenerateColumn(this));
            return;
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
            if (!isDirty || !built) return;
            this.Serialize();
            Serializer.SaveChunk(world.saveName, position, serialChunk);
            isDirty = false;
        }

        public async Task SaveThreaded() {
            if (!isDirty || !built) return;
            this.Serialize();
            await Task.Run(() => Serializer.SaveChunk(world.saveName, position, serialChunk));
            isDirty = false;
            return;
        }

        private bool InRange(int y) => y >= 0 && y < chunks.Length;
    }

}