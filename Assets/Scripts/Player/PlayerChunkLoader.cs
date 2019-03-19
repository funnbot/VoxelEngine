using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelEngine {

    public class PlayerChunkLoader : MonoBehaviour {
        public int range;

        private VoxelWorld world;

        private Coord2 pos = Coord2.zero;
        private Coord2 spiral = Coord2.zero;

        private List<Coord2> loaded;
        private List<Coord2> built;
        private List<Coord2> rendered;

        void Start() {
            loaded = new List<Coord2>();
            built = new List<Coord2>();
            rendered = new List<Coord2>();

            world = WorldManager.ActiveWorld;
            world.OnSpawnLoad += OnSpawnLoad;
        }

        void OnSpawnLoad() {
           // StartCoroutine(LoadChunks());
        }

        IEnumerator LoadChunks() {
            while (true) {
                yield return null;

                var cpos = (Coord2) Coord3.FloorToInt(transform.position).WorldToChunk();
                if (pos != cpos) spiral = Coord2.zero;
                pos = cpos;

                if (DestroyChunks()) continue;

                var load = pos + spiral;
                if (Coord2.SqrDistance(pos * Chunk.Size, load * Chunk.Size) > range * range) continue;

                //yield return BuildChunks(load);

                var column = world.GetColumn(load);
                if (!column.rendered) column.Render();

                SpiralOut(ref spiral);
            }
        }

        void CreateChunks() {
            foreach (Coord2 a in Coord2.Directions) {
                foreach (Coord2 b in Coord2.Directions) {
                    
                }
            }
        }

        bool DestroyChunks() {
            int len = loaded.Count();
            for (int i = len - 1; i >= 0; i--) {
                var p = loaded[i];
                if (Coord2.SqrDistance(pos * Chunk.Size, p * Chunk.Size) > range * range) {
                    world.DestroyColumn(p);
                    loaded.RemoveAt(i);
                    return true;
                }
            }
            return false;
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