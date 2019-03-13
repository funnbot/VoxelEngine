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

        void Start() {
            world.OnTick += OnTick;
            loadedChunks = new List<Coord2>();

            var s = Coord2.zero;
            for (int i = 0; i < 10; i++) {
                Debug.Log(s);
                SpiralOut(ref s);
            }
        }

        void OnTick() {
            var cpos = (Coord2) Coord3.FloorToInt(transform.position).WorldToChunk();
            if (pos != cpos) pos = spiral = Coord2.zero;
            pos = cpos;

            if (DestroyChunks()) return;

            if (!world.columns.ContainsKey(pos)) {
                LoadChunks(pos);
                return;
            }

            var c = world.GetColumn(pos);
            if (!c.rendered) {
                c.Render();
                return;
            }

            SpiralOut(ref spiral);
        }

        bool DestroyChunks() {
            int len = loadedChunks.Count();
            for (int i = len - 1; i >= 0; i--) {
                var p = loadedChunks[i];
                if (Coord2.Distance(pos, p) > range) {
                    world.DestroyColumn(p);
                    loadedChunks.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        void LoadChunks(Coord2 pos) {
            LoadChunk(pos);
            for (int i = 0; i < 4; i++)
                LoadChunk(pos + Coord2.Directions[i]);
        }
        void LoadChunk(Coord2 p) {
            if (world.columns.ContainsKey(p)) {
                world.LoadColumn(p).Build();
                loadedChunks.Add(p);
            }
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