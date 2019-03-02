using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class ChunkColumn : MonoBehaviour {
        public Chunk[] chunks;
        public Coord2 position { get; private set; }

        private VoxelWorld world;

        [SerializeField]
        private GameObject chunkFab;

        public Chunk GetChunk(int y) {
            if (InRange(y)) return chunks[y];
            else return null;
        }

        public void CleanUp() {
            foreach (var chunk in chunks)
                chunk.CleanUp();
        }

        public void Destroy() {
            foreach (var chunk in chunks)
                chunk.Destroy();
            chunks = null;
        }

        public void Build() {
            world.generator.GenerateChunks(position);
        }

        public void Render() {
            foreach (var chunk in chunks)
                chunk.Render();
        }

        public void Init(VoxelWorld world, Coord2 position) {
            this.position = position;
            this.world = world;

            transform.parent = world.transform;
            transform.localPosition = (Coord3)position * Chunk.Size;

            chunks = new Chunk[VoxelWorld.ChunkHeight];
            for (int i = 0; i < chunks.Length; i++) {
                var pos = new Coord3(position.x, i, position.y);

                chunks[i] = Instantiate(chunkFab).GetComponent<Chunk>();
                chunks[i].Init(this, world, pos);
            }
        }

        private bool InRange(int y) => y >= 0 && y < chunks.Length;
    }

}