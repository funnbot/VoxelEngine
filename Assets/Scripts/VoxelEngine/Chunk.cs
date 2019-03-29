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

        private VoxelWorld world;
        private ChunkSection[] chunks;

        public void Create(VoxelWorld world) {
            this.world = world;

            chunks = new ChunkSection[VoxelWorld.ChunkHeight];
            for (int i = 0; i < chunks.Length; i++) {
                chunks[i] = Instantiate(chunkFab).GetComponent<ChunkSection>();
                chunks[i].Create(this, world);
            }
        }

        public void Init(Coord2 position) {
            this.position = position;

            built = false;
            generated = false;
            rendered = false;

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

        public SerialChunk Serialized() {
            var serial = new SerialChunk();

            serial.blocks = new Block[VoxelWorld.ChunkHeight][][][];

            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].Serialize(serial, i);
            return serial;
        }

        public void Deserialize(SerialChunk serial) {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].Deserialize(serial, i);
        }

        public ChunkSection GetSection(int y) {
            if (InRange(y)) return chunks[y];
            else return null;
        }

        public void Build() {
            built = true;

            SerialChunk serial;
            if (Serializer.LoadChunk(world.saveName, position, out serial))
                this.Deserialize(serial);
            else world.generator.GenerateColumn(this);
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
            if (built) Serializer.SaveChunk(world.saveName, position, this.Serialized());
        }

        private bool InRange(int y) => y >= 0 && y < chunks.Length;
    }

}