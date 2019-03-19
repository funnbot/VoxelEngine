using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
using VoxelEngine.ProceduralGeneration;

public class WorldManager : MonoBehaviour {
    public GameObject WorldFab;
    public GameObject ColumnPoolFab;

    private VoxelWorld _active;
    public static VoxelWorld ActiveWorld { get => instance?._active; }

    public delegate void WorldSpawn(VoxelWorld world);
    public static event WorldSpawn OnWorldSpawn;

    void Start() {
        SpawnWorld("Sample", GeneratorType.Classic, 1338);
    }

    public static VoxelWorld SpawnWorld(string saveName, GeneratorType generator, int seed) {
        if (instance == null) return null;

        var world = Instantiate(instance.WorldFab).GetComponent<VoxelWorld>();
        instance._active = world;

        world.columnPool = Instantiate(instance.ColumnPoolFab).GetComponent<ChunkColumnPool>();

        world.saveName = saveName;
        world.name = saveName;
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