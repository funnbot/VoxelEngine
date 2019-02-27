using UnityEngine;

namespace VoxelEngine {

    public class Chunk : MonoBehaviour {
        public const int Size = 16;
        public const int Rollover = 0;

        public bool update;
        public bool built;

        public float BlockUVTileSize;
        public float TransUVTileSize;

        public MeshFilter BlockRend;
        public MeshFilter TransRend;
        public Transform StandaloneBlocks;
        public MeshCollider BlockCollider;

        public VoxelWorld world;
        public Vector3Int position;
        public Vector3Int worldPosition;

        private Block[, , ] blocks;

        private MeshData blockMesh;
        private MeshData transMesh;
        private MeshData colliderMesh;

        private Chunk[] neighbors;

        public void Init(VoxelWorld world, Vector3Int position) {
            blocks = new Block[Chunk.Size, Chunk.Size, Chunk.Size];
            this.world = world;
            this.position = position;
            worldPosition = position * Chunk.Size;

            blockMesh = new MeshData();
            transMesh = new MeshData();
            colliderMesh = new MeshData();

            HookEvents();
        }

        public void OnTick() {
            if (update) Render();
        }

        public void Destroy() {
            world.OnTick -= OnTick;
        }

        public void Build() {
            // TODO: load from saves
            world.generator.GenerateChunks(position);
            built = true;
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

        public Block GetBlock(Vector3Int pos) {
            if (InRange(pos)) return blocks[pos.x, pos.y, pos.z];
            else {
                pos = BlockToWorldPos(pos);
                return world.GetBlock(pos);
            }
        }
        public void SetBlock(Vector3Int pos, Block block) {
            if (InRange(pos)) blocks[pos.x, pos.y, pos.z] = block;
            else {
                pos = BlockToWorldPos(pos);
                world.SetBlock(pos, block, false);
            }
        }

        public Vector3Int BlockToWorldPos(Vector3Int block) =>
            block + position * Chunk.Size;
        public Vector3Int WorldToBlockPos(Vector3Int block) =>
            block - position * Chunk.Size;
        public Vector3Int TransformChunkPos(Vector3Int pos, Vector3Int chunkWorldPos) =>
            pos + worldPosition - chunkWorldPos;

        public bool InRange(int x, int y, int z) =>
            x >= 0 && x < Size && y >= 0 && y < Size && z >= 0 && z < Size;
        public bool InRange(Vector3Int p) => InRange(p.x, p.y, p.z);

        void HookEvents() {
            world.OnTick += OnTick;
        }

        void AddBlockToMesh(int x, int y, int z) {
            var pos = new Vector3Int(x, y, z);
            var block = blocks[x, y, z];

            if (block.data.meshType == BlockMeshType.Cube) {
                for (int i = 0; i < 6; i++) {
                    
                    if (!CullFace(block, pos, i)) continue;

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

        private bool CullFace(Block block, Vector3Int pos, int dir) {
            if (block.data.transparent) return true;
            var adjPos = pos + DirOffsets[dir];
            var adjacent = InRange(adjPos) ? GetBlock(adjPos) :
                neighbors[dir]?.GetBlock(TransformChunkPos(adjPos, neighbors[dir].worldPosition));
            return adjacent != null && adjacent.data.transparent;
        }

        private void FetchNeighbors() {
            neighbors = new Chunk[6];
            for (int i = 0; i < 6; i++) {
                var pos = position + DirOffsets[i];
                neighbors[i] = world.GetChunk(pos);
            }
        }

        public static Vector3Int[] DirOffsets = {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.right,
            Vector3Int.left,
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

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