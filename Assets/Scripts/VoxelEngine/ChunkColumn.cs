using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class ChunkColumn : MonoBehaviour {
        public Chunk[] chunks;
        public Coord2 position { get; private set; }

        public bool built { get; private set; }
        public bool rendered { get; private set; }

        private VoxelWorld world;

        [SerializeField]
        private GameObject chunkFab;

        public Chunk GetChunk(int y) {
            if (InRange(y)) return chunks[y];
            else return null;
        }

        public void Create(VoxelWorld world) {
            this.world = world;
            built = false;
            rendered = false;

            chunks = new Chunk[VoxelWorld.ChunkHeight];
            for (int i = 0; i < chunks.Length; i++) {
                chunks[i] = Instantiate(chunkFab).GetComponent<Chunk>();
                chunks[i].Create(this, world);
            }
        }

        public void Init(Coord2 position) {
            this.position = position;

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

        public void Build() {
            built = true;
            world.generator.GenerateChunks(position);
        }

        public void Render() {
            rendered = true;
            foreach (var chunk in chunks)
                chunk.Render();
        }

        private bool InRange(int y) => y >= 0 && y < chunks.Length;
    }

}