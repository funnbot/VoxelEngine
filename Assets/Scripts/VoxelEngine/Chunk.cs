using UnityEngine;

namespace VoxelEngine {

    public class Chunk : MonoBehaviour {
        public static readonly int Size = 16;
        public static readonly int Rollover = 0;

        public bool update;
        public bool built;

        public MeshFilter BlockRend;
        public MeshFilter TransRend;
        public Transform StandaloneBlocks;
        public MeshCollider BlockCollider;

        public VoxelWorld world;
        public Coord3 position;
        public Coord3 worldPosition;

        private Block[, , ] blocks;

        private MeshData blockMesh;
        private MeshData transMesh;
        private MeshData colliderMesh;

        private ChunkColumn parent;
        private Chunk[] neighbors;

        public void Init(ChunkColumn parent, VoxelWorld world, Coord3 position) {
            blocks = new Block[Chunk.Size, Chunk.Size, Chunk.Size];

            this.world = world;
            this.position = position;
            this.parent = parent;

            transform.parent = parent.transform;
            transform.localPosition = Coord3.up * position * Size;

            worldPosition = position * Chunk.Size;

            blockMesh = new MeshData();
            transMesh = new MeshData();
            colliderMesh = new MeshData();

            HookEvents();
        }

        public void OnTick() {
            if (update) Render();
        }

        public void CleanUp() {
            world.OnTick -= OnTick;
            blocks = null;
            blockMesh.Clear();
            transMesh.Clear();
            colliderMesh.Clear();
        }

        public void Destroy() {
            world.OnTick -= OnTick;
            blocks = null;
            blockMesh.Clear();
            transMesh.Clear();
            colliderMesh.Clear();
        }

        public void Render() {
            update = false;

            blockMesh.Clear();
            transMesh.Clear();
            colliderMesh.Clear();

            FetchNeighbors();

            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    for (int z = 0; z < Size; z++) {
                        AddBlockToMesh(x, y, z);
                    }
                }
            }

            BlockRend.sharedMesh = blockMesh.ToMesh();
            TransRend.sharedMesh = transMesh.ToMesh();

            BlockCollider.sharedMesh = colliderMesh.ToColMesh();
        }

        public Block GetBlock(Coord3 pos) {
            if (InRange(pos)) return blocks[pos.x, pos.y, pos.z];
            else {
                pos = BlockToWorldPos(pos);
                return world.GetBlock(pos);
            }
        }
        public void SetBlock(Coord3 pos, Block block, bool update = true) {
            if (InRange(pos)) {
                blocks[pos.x, pos.y, pos.z] = block;
                if (update) {
                    this.update = true;
                    UpdateNeighbors(pos);
                }
            } else {
                pos = BlockToWorldPos(pos);
                world.SetBlock(pos, block, false);
            }
        }

        public void UpdateNeighbors(Coord3 changed) {
            if (changed.InRange(1, Chunk.Size - 1)) return;
            FetchNeighbors();
            for (int i = 0; i < 6; i++) {
                var pos = changed + Coord3.Directions[i];
                if (!pos.InRange(0, Chunk.Size)) {
                    Chunk n = neighbors[i];
                    if (n == null) continue;
                    n.update = true;
                }
            }
        }

        public Coord3 BlockToWorldPos(Coord3 block) =>
            block + position * Chunk.Size;
        public Coord3 WorldToBlockPos(Coord3 block) =>
            block - position * Chunk.Size;
        public Coord3 TransformChunkPos(Coord3 pos, Coord3 chunkWorldPos) =>
            pos + worldPosition - chunkWorldPos;

        public bool InRange(int x, int y, int z) =>
            x >= 0 && x < Size && y >= 0 && y < Size && z >= 0 && z < Size;
        public bool InRange(Coord3 p) => InRange(p.x, p.y, p.z);

        void HookEvents() {
            world.OnTick += OnTick;
        }

        void AddBlockToMesh(int x, int y, int z) {
            var pos = new Coord3(x, y, z);
            var block = blocks[x, y, z];

            if (block.data.meshType == BlockMeshType.Cube) {
                for (int i = 0; i < 6; i++) {

                    var adjPos = Coord3.Directions[i] + pos;
                    //var adjacent = InRange(adjPos) ? GetBlock(adjPos) :
                    //    neighbors[dir]?.GetBlock(TransformChunkPos(adjPos, neighbors[dir].worldPosition));
                    var adjacent = GetBlock(adjPos);
                    if (!block.data.transparent && adjacent != null && !adjacent.data.transparent) continue;

                    int texIndex = TextureIndex(block.data.texIndices, i);
                    if (block.data.transparent) {
                        transMesh.AddQuad(i, pos, texIndex);
                    } else {
                        blockMesh.AddQuad(i, pos, texIndex);
                    }
                    colliderMesh.AddQuad(i, pos, texIndex);
                }
            } else if (block.data.meshType == BlockMeshType.Decal) {
                var texIndex = TextureIndex(block.data.texIndices, 0);
                transMesh.AddDecal(pos, texIndex);
            }
        }

        private int TextureIndex(int[] inds, int dir) {
            if (inds.Length == 1) return inds[0];
            else if (inds.Length == 3) return inds[Mathf.Min(dir, 2)];
            else return inds[dir];
        }

        /* private bool CullFace(Block block, Coord3 pos, int dir) {

        }*/

        private void FetchNeighbors() {
            neighbors = new Chunk[6];
            for (int i = 0; i < 6; i++) {
                var pos = position + Coord3.Directions[i];
                neighbors[i] = world.GetChunk(pos);
            }
        }

        public static class Direction {
            public static int Up = 0;
            public static int Down = 1;
            public static int Right = 2;
            public static int Left = 3;
            public static int Forward = 4;
            public static int Backward = 5;
        }
    }

}