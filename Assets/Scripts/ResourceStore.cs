using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

// Singleton
public class ResourceStore : MonoBehaviour {
    public static Resource<BlockData> Blocks;
    public static Resource<StructureData> Structures;

    private void Load() {
        Blocks = new Resource<BlockData>("Blocks", new [] { "air", "stone", "grass", "dirt" });

        Structures = new Resource<StructureData>("Structures");
    }

    public static ResourceStore Inst;
    void Awake() {
        if (Inst != null) {
            Destroy(Inst.gameObject);
            Debug.LogError("Destroying duplicate of singleton Resource");
            return;
        }
        Inst = this;
        Load();
    }

}

public class Resource<T> : IEnumerable where T : ScriptableObject {
    Dictionary<string, T> resources;

    public T this [string id] {
        get {
            T v;
            if (resources.TryGetValue(id, out v))
                return v;
            else return null;
        }
    }

    public Resource(string folder, string[] required = null) {
        resources = new Dictionary<string, T>();
        var loaded = Resources.LoadAll(folder, typeof(T));
        foreach (T r in loaded) resources.Add(r.name, r);

        if (required != null) {
            foreach (var r in required)
                if (!resources.ContainsKey(r))
                    Debug.LogError("Missing required " + folder + " resource: " + r);
        }
    }

    public IEnumerator GetEnumerator() => resources.GetEnumerator();
}