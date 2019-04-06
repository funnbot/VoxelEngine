using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Internal;

namespace VoxelEngine.Player {

    public class PlayerChunkLoader : MonoBehaviour {
        public int range;

        private VoxelWorld world;

        private Coord2 currentPos = Coord2.zero;
        private Coord2 spiralPos = Coord2.zero;

        private List<Coord2> loaded;
        private bool spawnLoaded;

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
                yield return LoadChunks().WaitTillComplete();
            }
        }

        async Task LoadChunks() {
            var chunkPos = (Coord2) Coord3.FloorToInt(transform.position).WorldToChunk();
            if (currentPos != chunkPos) spiralPos = Coord2.zero;
            currentPos = chunkPos;
            var toLoad = currentPos + spiralPos;

            if (ChunkInRange(toLoad, range - 32)) {
                DestroyOldChunks();

                var chunk = LoadChunks(toLoad);
                await chunk.BuildTerrain();
                await chunk.GenerateMesh();
                chunk.ApplyMesh();
                await chunk.UpdateNeighbors();

                SpiralOut(ref spiralPos);
            }
        }

        Chunk LoadChunks(Coord2 pos) {
            if (!world.chunks.ContainsChunk(pos)) loaded.Add(pos);
            var chunk = world.chunks.CreateChunk(pos);
            for (int i = 0; i < 4; i++) {
                var p = pos + Coord2.Directions[i];
                if (!world.chunks.ContainsChunk(p)) {
                    loaded.Add(p);
                    world.chunks.CreateChunk(p);
                }
            }
            return chunk;
        }

        async void DestroyOldChunks() {
            int count = loaded.Count;
            for (int i = count - 1; i >= 0; i--) {
                var pos = loaded[i];
                if (!ChunkInRange(pos, range)) {
                    var chunk = world.chunks.GetChunk(pos);
                    await chunk.SaveBlocks();
                    world.chunks.DeleteChunk(chunk);
                    loaded.RemoveAt(i);
                    return;
                }
            }
        }

        bool ChunkInRange(Coord2 col, int range) =>
            Coord2.SqrDistance(currentPos * ChunkSection.Size, col * ChunkSection.Size) < range * range;

        void SpiralOut(ref Coord2 c) {
            if (c.x >= 0 && c.y == 0) c += Coord2.up;
            else if (c.x > 0 && c.y >= 0) c += Coord2.topLeft;
            else if (c.x <= 0 && c.y > 0) c += Coord2.bottomLeft;
            else if (c.x < 0 && c.y <= 0) c += Coord2.bottomRight;
            else if (c.x >= 0 && c.y < 0) c += Coord2.topRight;
        }
    }
}