using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine.ProceduralGeneration {

    public class ProceduralGenerator {
        Dictionary<GeneratorType, Generator> generators;
        SimplexNoise noise;

        public ProceduralGenerator(VoxelWorld world) {
            noise = new SimplexNoise(world.seed);
            generators = new Dictionary<GeneratorType, Generator>();

            generators.Add(GeneratorType.Classic, new ClassicGenerator(world, noise));
            generators.Add(GeneratorType.Flat, new FlatGenerator(world, noise));
            generators.Add(GeneratorType.Noise, new NoiseGenerator(world, noise));
        }

        public Generator Use(GeneratorType type) =>
            generators[type];
    }

    public enum GeneratorType {
        Classic,
        Flat,
        Noise
    }
}