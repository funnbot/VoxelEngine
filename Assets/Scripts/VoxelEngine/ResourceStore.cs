using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Data;

namespace VoxelEngine {

    // Singleton
    public class ResourceStore : SingletonMonoBehaviour<ResourceStore> {
        public static BlockResource Blocks;
        public static Resource<StructureData> Structures;

        protected override void AwakeImpl() {
            Blocks = new BlockResource("Blocks", new [] { "stone", "grass", "dirt" });

            Structures = new Resource<StructureData>("Structures");
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

    public class BlockResource : IEnumerable {
        Dictionary<byte, BlockData> blocks;
        Dictionary<string, byte> blockMap;

        public BlockData this [string id] {
            get {
                byte key;
                if (!blockMap.TryGetValue(id, out key)) return null;
                return this [key];
            }
        }

        public BlockData this [byte id] {
            get {
                BlockData v;
                blocks.TryGetValue(id, out v);
                return v;
            }
        }

        public BlockResource(string folder, string[] required = null) {
            blocks = new Dictionary<byte, BlockData>();
            blockMap = new Dictionary<string, byte>();

            byte id = 0;
            var loaded = Resources.LoadAll(folder, typeof(BlockData));
            foreach (BlockData r in loaded) {
                r.blockId = r.name;
                r.byteId = id++;
                blocks.Add(r.byteId, r);
                blockMap.Add(r.blockId, r.byteId);
            }

            if (required != null) {
                foreach (var r in required)
                    if (!blockMap.ContainsKey(r))
                        Debug.LogError("Missing required " + folder + " resource: " + r);
            }
        }

        public IEnumerator GetEnumerator() => blocks.GetEnumerator();
    }

}