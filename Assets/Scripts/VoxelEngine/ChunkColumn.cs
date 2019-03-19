using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class ChunkColumn : MonoBehaviour {
        public GameObject chunkFab;

        public Coord2 position { get; private set; }

        public bool built { get; private set; }
        public bool generated { get; private set; }
        public bool rendered { get; private set; }

        private VoxelWorld world;
        private Chunk[] chunks;

        public void Create(VoxelWorld world) {
            this.world = world;

            chunks = new Chunk[VoxelWorld.ChunkHeight];
            for (int i = 0; i < chunks.Length; i++) {
                chunks[i] = Instantiate(chunkFab).GetComponent<Chunk>();
                chunks[i].Create(this, world);
            }
        }

        public void Init(Coord2 position) {
            this.position = position;

            built = false;
            generated = false;
            rendered = false;

            transform.parent = world.transform;
            transform.localPosition = (Coord3) position * Chunk.Size;

            for (int i = 0; i < chunks.Length; i++) {
                var pos = new Coord3(position.x, i, position.y);
                chunks[i].Init(pos);
            }
        }

        public void CleanUp() {
            foreach (var chunk in chunks)
                chunk.CleanUp();
        }

        public SerialChunkColumn Serialize() {
            var serial = new SerialChunkColumn();

            serial.blocks = new Block[VoxelWorld.ChunkHeight][][][];

            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].Serialize(serial, i);
            return serial;
        }

        public void Deserialize(SerialChunkColumn serial) {
            for (int i = 0; i < VoxelWorld.ChunkHeight; i++)
                chunks[i].Deserialize(serial, i);
        }

        public Chunk GetChunk(int y) {
            if (InRange(y)) return chunks[y];
            else return null;
        }

        public void Build() {
            built = true;

            SerialChunkColumn serial;
            if (Serializer.LoadColumn(world.saveName, position, out serial))
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

        public void Render() {
            generated = true;
            rendered = true;
            foreach (var chunk in chunks)
                chunk.Render();
        }

        public void Save() {
            if (built) Serializer.SaveColumn(world.saveName, position, this.Serialize());
        }

        private bool InRange(int y) => y >= 0 && y < chunks.Length;
    }

}