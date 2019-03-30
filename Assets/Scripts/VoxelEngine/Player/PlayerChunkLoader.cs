using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelEngine.Player {

    public class PlayerChunkLoader : MonoBehaviour {
        public int range;

        private VoxelWorld world;

        private Coord2 pos = Coord2.zero;
        private Coord2 spiral = Coord2.zero;

        private List<Coord2> loaded;

        void Start() {
            loaded = new List<Coord2>();

            WorldManager.OnWorldSpawn += OnWorldSpawn;
        }

        void OnWorldSpawn(VoxelWorld world) {
            this.world = world;
            world.OnSpawnLoad += OnSpawnLoad;
        }

        void OnSpawnLoad() {
            StartCoroutine(LoadChunksRoutine());
        }

        IEnumerator LoadChunksRoutine() {
            while (true) {
                var t = LoadChunk();
                do yield return null;
                while (t.Status != TaskStatus.RanToCompletion);
            }
        }

        async Task LoadChunk() {
            var cpos = (Coord2) Coord3.FloorToInt(transform.position).WorldToChunk();
            if (pos != cpos) spiral = Coord2.zero;
            pos = cpos;
            var load = pos + spiral;

            if (await DestroyChunks()) return;

            if (!ColumnInRange(load, range - 32)) return;

            LoadColumns(load);
            var column = world.chunks.GetChunk(load);

            try {
                if (!column.built) {
                    await column.BuildThreaded();
                }
                if (!column.generated) {
                    await Task.Run(column.GenerateMesh);
                    foreach (var dir in Coord2.Directions) {
                        var col = world.chunks.GetChunk(load + dir);
                        if (col != null && col.generated) {
                            await Task.Run(col.GenerateMesh);
                        }
                    }
                }
            } catch (System.Exception e) {
                Debug.Log(e);
            }
            if (!column.rendered) {
                column.ApplyMesh();
                foreach (var dir in Coord2.Directions) {
                    var col = world.chunks.GetChunk(load + dir);
                    if (col != null && col.generated && col.rendered) {
                        col.ApplyMesh();
                    }
                }
            }

            SpiralOut(ref spiral);
            return;
        }

        void LoadColumns(Coord2 pos) {
            LoadColumn(pos);
            foreach (var dir in Coord2.Directions) {
                LoadColumn(pos + dir);
            }
        }

        void LoadColumn(Coord2 pos) {
            if (world.chunks.ContainsChunk(pos)) return;
            world.chunks.LoadChunk(pos);
            loaded.Add(pos);
        }

        async Task<bool> DestroyChunks() {
            int len = loaded.Count();
            for (int i = len - 1; i >= 0; i--) {
                var p = loaded[i];
                if (!ColumnInRange(p, range)) {
                    var col = world.chunks.GetChunk(p);
                    if (col != null) {
                        try {
                            await col.SaveThreaded();
                        } catch (System.Exception e) {
                            Debug.Log(e);
                        }
                        world.chunks.DestroyChunk(p);
                    }
                    loaded.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        bool ColumnInRange(Coord2 col, int range) =>
            Coord2.SqrDistance(pos * ChunkSection.Size, col * ChunkSection.Size) < range * range;

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