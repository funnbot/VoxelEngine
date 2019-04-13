using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.Blocks {

    public class StandaloneBlock : Block {
        public GameObject gameObject;

        public StandaloneBlock() { }

        public override void OnLoad(Coord3 pos, BlockData data, ChunkSection chunk) {
            gameObject = Chunk.Instantiate(data.prefab, chunk.Blocks);
            gameObject.transform.localPosition = pos;
            gameObject.name = data.blockId + " " + pos;
        }

        public override void OnUnload() {
            if (gameObject != null) Chunk.Destroy(gameObject, 0.05f);
        }
    }

}