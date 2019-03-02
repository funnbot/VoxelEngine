using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class ChunkLoader : MonoBehaviour {
        public VoxelWorld world;
        public int range;

        private Coord3 pos = Coord3.zero;
        private Coord3 spiral = Coord3.zero;

        private List<Coord2> loadedChunks;

        void Start() {
            // world.OnTick += OnTick;
            loadedChunks = new List<Coord2>();
        }

        void OnTick() {
            var chunkPos = world.BlockToChunkPos(Coord3.FloorToInt(transform.position));
            chunkPos.y = 0;
            if (chunkPos != pos) spiral = Coord3.zero;
            var p = pos + spiral;
            var chunk = world.GetChunk(p);
            if (chunk == null) {
               // world.LoadChunks(new Vector2Int(p.x, p.z));
               // world.BuildChunks(new Vector2Int(p.x, p.z));
                chunk = world.GetChunk(p);
            } else if (chunk.built) {
               // world.RenderChunks(new Vector2Int(p.x, p.z));
                loadedChunks.Add(new Coord2(p.x, p.z));
                //for (int i = 2; i < 6; i++) world.RenderChunks(new Vector2Int(DirOffsets));
                Spiral();

                DestroyChunks();
            }

            pos = chunkPos;
        }

        void DestroyChunks() {
            for (int i = 0; i < loadedChunks.Count; i++) {
                var p = loadedChunks[i];
                var chunk = new Coord3(p.x, 0, p.y);
                if (Coord3.Distance(pos, chunk) > range) {
                    // world.DestroyChunks(p);
                    return;
                }

            }
        }

        void Spiral() {
            int x = spiral.x, y = spiral.z;
            if (x == y) {
                if (x >= 0) x++;
                else y++;
            } else if (-x == y) {
                if (x <= 0) x++;
                else x--;
            } else if (y >= 0) {
                if (Mathf.Abs(x) < y) x++;
                else {
                    if (x >= 0) y--;
                    else y++;
                }
            } else {
                if (Mathf.Abs(x) < -y) x--;
                else {
                    if (x >= 0) y--;
                    else y++;
                }
            }
            spiral = new Coord3(x, 0, y);
        }
    }

}