using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.Blocks {

    public class StandaloneBlock : CustomBlock {
        public StandaloneBlock(byte id) : base(id) { }

        public GameObject gameObject;

        public override void OnLoad() {
            gameObject = Chunk.Instantiate(data.prefab, chunk.Blocks);
            gameObject.transform.localPosition = position;
            gameObject.name = data.blockId + " " + position;
        }

        public override void OnUnload() {
            if (gameObject != null) Chunk.Destroy(gameObject, 0.05f);
        }
    }

}