using System.Collections.Generic;
using OpenSimplexNoise;

namespace VoxelEngine.TerrainGeneration {

    public class ProceduralGenerator {
        VoxelWorld world;
        Dictionary<GeneratorType, Generator> generators;
        SimplexNoise noise;

        public ProceduralGenerator(VoxelWorld world) {
            this.world = world;
            noise = new SimplexNoise(world.seed);
            generators = new Dictionary<GeneratorType, Generator>();

            generators.Add(GeneratorType.Classic, new ClassicGenerator(world, noise));
            generators.Add(GeneratorType.Flat, new FlatGenerator(world, noise));
            generators.Add(GeneratorType.Noise, new NoiseGenerator(world, noise));
            generators.Add(GeneratorType.Test, new TestGenerator(world, noise));
        }

        public Generator Use(GeneratorType type) =>
            generators[type];
    }

    public enum GeneratorType {
        Classic,
        Flat,
        Noise,
        Test
    }
}