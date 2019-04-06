using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;

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
        BlockData[] blocks;
        Dictionary<string, byte> blockMap;

        public BlockData this [string id] {
            get {
                byte key;
                if (!blockMap.TryGetValue(id, out key)) return null;
                return blocks[key];
            }
        }

        public BlockData this [byte id] {
            get => blocks[id];
        }

        public BlockData GetData(byte id) => blocks[id];
        public byte GetId(string name) => blockMap[name];

        public BlockResource(string folder, string[] required = null) {
            var loaded = Resources.LoadAll(folder, typeof(BlockData));

            blocks = new BlockData[loaded.Length];
            blockMap = new Dictionary<string, byte>();

            for (int i = 0; i < loaded.Length; i++) {
                var block = (BlockData) loaded[i];
                var id = block.id;
                block.blockId = block.name;
                blockMap.Add(block.blockId, (byte) id);
                blocks[id] = block;
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