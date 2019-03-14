using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class ChunkLoader : MonoBehaviour {
        public VoxelWorld world;
        public int range;

        private Coord2 pos = Coord2.zero;
        private Coord2 spiral = Coord2.zero;

        private List<Coord2> loadedChunks;

        void Awake() {
            loadedChunks = new List<Coord2>();
            world.OnSpawnLoad += OnSpawnLoad;
        }

        void OnSpawnLoad() {
            StartCoroutine(LoadChunks());
        }

        IEnumerator LoadChunks() {
            while (true) {
                yield return null;

                var cpos = (Coord2) Coord3.FloorToInt(transform.position).WorldToChunk();
                if (pos != cpos) pos = spiral = Coord2.zero;
                pos = cpos;

                if (DestroyChunks()) continue;
                var load = pos + spiral;
                if (Coord2.Distance(pos * Chunk.Size, load * Chunk.Size) > range) continue;

                yield return BuildChunks(load);

                world.GetColumn(load).Render();

                SpiralOut(ref spiral);

                yield return null;
            }
        }

        bool DestroyChunks() {
            int len = loadedChunks.Count();
            for (int i = len - 1; i >= 0; i--) {
                var p = loadedChunks[i];
                if (Coord2.Distance(pos * Chunk.Size, p * Chunk.Size) > range) {
                    world.DestroyColumn(p);
                    loadedChunks.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        IEnumerator BuildChunks(Coord2 pos) {
            BuildChunk(pos);
            for (int i = 0; i < 4; i++) {
                BuildChunk(pos + Coord2.Directions[i]);
                yield return null;
            }
        }
        void BuildChunk(Coord2 p) {
            ChunkColumn column;
            if (!world.columns.ContainsKey(p)) {
                column = world.LoadColumn(p);
                loadedChunks.Add(p);
            } else column = world.GetColumn(p);
            column.Build();
        }

        void SpiralOut(ref Coord2 c) {
            int x = c.x, y = c.y;
            if (x >= 0 && y == 0) y++;

            else if (x > 0 && y >= 0) {
                x--;
                y++;
            } else if (x <= 0 && y > 0) {
                x--;
                y--;
            } else if (x < 0 && y <= 0) {
                x++;
                y--;
            } else if (x >= 0 && y < 0) {
                x++;
                y++;
            }

            c.x = x;
            c.y = y;
        }

        void Spiral() {
            int x = spiral.x, y = spiral.y;
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
            spiral = new Coord2(x, y);
        }
    }

}

//  Coord  |  X>Y  | Sign
//  0,0        
//  1,0
//  0,1
//  -1,0
//  0,-1
//  
//
//
//
//
//
//