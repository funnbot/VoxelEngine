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

    void Start() {
        SpawnWorld("Sample", GeneratorType.Classic, 1337);
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

        return world;
    }

    private static WorldManager instance;
    void Awake() {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }
}