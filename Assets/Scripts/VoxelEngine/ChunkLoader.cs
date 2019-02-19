using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class ChunkLoader : MonoBehaviour {
        public VoxelWorld world;
        public int range;

        
        public Texture2D tex;
        public double freq;
        public int size;

        int tick;
        void Start() {
            var chunkPos = world.BlockToChunkPos(Vector3Int.FloorToInt(transform.position));
        }

        [ContextMenu("Generate")]
        void Generate() {
            var n = new SimplexNoise();
            tex = new Texture2D(size, size);
            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    float f = (float)n.Evaluate(freq * x, 0, freq * y);
                    f = (f + 1f) / 2f;
                    tex.SetPixel(x, y, new Color(f, f, f, 1));
                }
            }
            tex.Apply();
        }

        void Update() {

        }
    }

}