using VoxelEngine.Internal;

namespace VoxelEngine.Pooling {

    public class ChunkPool : PrefabPool<Chunk> {
        public override void CleanUp(Chunk col) {
            col.CleanUp();
            col.transform.parent = transform;
        }

        public override Chunk Create() {
            var col = Instantiate(prefab).GetComponent<Chunk>();
            col.Create(WorldManager.ActiveWorld);
            col.transform.parent = transform;
            return col;
        }
    }

}