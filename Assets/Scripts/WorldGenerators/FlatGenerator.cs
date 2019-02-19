using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.ProceduralGeneration {

    public class FlatGenerator : Generator {
        int stoneHeight = 20;
        int dirtHeight = 4;
        float treeFrequency = 0.7f;
        int treeDensity = 15;
        float grassFrequency = 0.4f;
        int grassDensity = 24;
        float caveFrequency = 0.05f;
        int caveDensity = 20;

        public FlatGenerator(VoxelWorld world, SimplexNoise noise) : base(world, noise) {
            
        }

        protected override void GenerateColumn(Chunk chunk, int x, int z) {
            
        }
    }

}