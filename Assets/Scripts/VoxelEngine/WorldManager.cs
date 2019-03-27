using UnityEngine;
using VoxelEngine.Pooling;
using VoxelEngine.ProceduralGeneration;

namespace VoxelEngine {

    public class WorldManager : MonoBehaviour {
        public GameObject WorldFab;
        public GameObject ColumnPoolFab;

        private VoxelWorld _active;
        public static VoxelWorld ActiveWorld { get => instance?._active; }

        public delegate void WorldSpawn(VoxelWorld world);
        public static event WorldSpawn OnWorldSpawn;

        void Start() {
            SpawnWorld("Sample", GeneratorType.Classic, System.DateTime.Now.Millisecond);
        }

        public static VoxelWorld SpawnWorld(string saveName, GeneratorType generator, int seed) {
            if (instance == null) return null;

            var world = Instantiate(instance.WorldFab).GetComponent<VoxelWorld>();
            instance._active = world;

            world.columnPool = Instantiate(instance.ColumnPoolFab).GetComponent<ChunkPool>();

            world.saveName = saveName;
            world.name = saveName + " (VoxelWorld)";
            world.generatorType = generator;
            world.seed = seed;

            OnWorldSpawn?.Invoke(world);

            // System.IO.Directory.Delete($"Worlds/{world.saveName}", true);

            return world;
        }

        private static WorldManager instance;
        void Awake() {
            if (instance != null) Destroy(gameObject);
            else instance = this;
        }
    }

}