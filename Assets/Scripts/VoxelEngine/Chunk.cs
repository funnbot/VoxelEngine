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

        public void Create(ChunkColumn parent, VoxelWorld world) {
            blocks = new Block[Chunk.Size, Chunk.Size, Chunk.Size];

            this.world = world;
            this.parent = parent;

            transform.parent = parent.transform;

            blockMesh = new MeshData();
            transMesh = new MeshData();
            colliderMesh = new MeshData();
        }

        public void Init(Coord3 position) {
            blocks = new Block[Size, Size, Size];

            this.position = position;
            worldPosition = position * Size;
            transform.localPosition = Coord3.up * worldPosition;

            world.OnTick += OnTick;
        }

        public void CleanUp() {
            world.OnTick -= OnTick;
            blocks = null;
            blockMesh.Clear();
            transMesh.Clear();
            colliderMesh.Clear();
        }

        void OnTick() {
            if (update) Render();
        }

        public void Render() {
            Debug.Log("Rendered Chunk: " + position);

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
            if (pos.InRange(0, Chunk.Size)) return blocks[pos.x, pos.y, pos.z];
            else return world.GetBlock(pos.WorldToBlock(worldPosition));
        }
        public void SetBlock(Coord3 pos, Block block, bool update = true) {
            if (pos.InRange(0, Chunk.Size)) {
                world.UnregisterBlock(blocks[pos.x, pos.y, pos.z]);
                blocks[pos.x, pos.y, pos.z] =
                    world.RegisterBlock(block, pos.BlockToWorld(worldPosition), this);

                if (update) {
                    Render();
                    UpdateNeighbors(pos);
                }
            } else world.SetBlock(pos.BlockToWorld(worldPosition), block, update);
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

        void AddBlockToMesh(int x, int y, int z) {
            var pos = new Coord3(x, y, z);
            var block = blocks[x, y, z];

            if (block.data.meshType == BlockMeshType.Cube) {
                for (int i = 0; i < 6; i++) {
                    // if (block.data.transparent) return false;
                    var adjPos = Coord3.Directions[i] + pos;
                    var adjacent = adjPos.InRange(0, Chunk.Size) ? GetBlock(adjPos) :
                        neighbors[i]?.GetBlock(adjPos.TransformChunk(worldPosition, neighbors[i].worldPosition));

                    if (pos == Coord3.zero) {
                        Debug.Log("Adjacent: " + adjPos + " " + (adjacent == null));
                    }
                    
                    if (!block.data.transparent && adjacent != null && !adjacent.data.transparent) continue;

                    var rot = Rotate(i, block.rotation);
                    int texIndex = block.data.textureIndices[rot.index];
                    if (block.data.transparent)
                        transMesh.AddQuad(i, pos, rot.face, texIndex);
                    else blockMesh.AddQuad(i, pos, rot.face, texIndex);
                    colliderMesh.AddQuad(i, pos);
                }
            } else if (block.data.meshType == BlockMeshType.Decal) {
                var texIndex = block.data.textureIndices[0];
                transMesh.AddDecal(pos, texIndex);
            }
        }

        private static readonly int[] zRot = { BlockFace.top, BlockFace.left, BlockFace.bottom, BlockFace.right },
            yRot = { BlockFace.front, BlockFace.right, BlockFace.back, BlockFace.left },
            xRot = { BlockFace.front, BlockFace.top, BlockFace.back, BlockFace.bottom };

        private static readonly int[][] zFace = { new [] { 3, 1, 3, 3, 3, 3 }, new [] { 2, 2, 2, 2, 2, 2, }, new [] { 1, 3, 1, 1, 1, 1 } },
            yFace = { new [] { 0, 0, 3, 1, 0, 0 }, new [] { 0, 0, 2, 2, 0, 0 }, new [] { 0, 0, 1, 3, 0, 0, } },
            xFace = { new [] { 0, 2, 0, 2, 1, 3 }, new [] { 2, 2, 0, 2, 2, 2 }, new [] { 0, 2, 2, 0, 3, 1 } };

        public static(int index, int face) Rotate(int dir, Coord3 rot) {
            rot = rot % 4;
            int frot = 0;
            if (rot.z != 0) {
                if (dir != BlockFace.front && dir != BlockFace.back) {
                    var ind = zRot.IndexOf(dir);
                    dir = zRot[(rot.z + ind + 4) % 4];
                }
                frot += zFace[rot.z < 0 ? 3 + rot.z : rot.z - 1][dir];
            }
            if (rot.y != 0) {
                if (dir != BlockFace.top && dir != BlockFace.bottom) {
                    var ind = yRot.IndexOf(dir);
                    dir = yRot[(rot.y + ind + 4) % 4];
                }
                frot += yFace[rot.y < 0 ? 3 + rot.y : rot.y - 1][dir];
            }
            if (rot.x != 0) {
                if (dir != BlockFace.right && dir != BlockFace.left) {
                    var ind = xRot.IndexOf(dir);
                    dir = xRot[(rot.x + ind + 4) % 4];
                }
                frot += xFace[rot.x < 0 ? 3 + rot.x : rot.x - 1][dir];
            }
            return (index: dir, face: frot);
        }

        private void FetchNeighbors() {
            neighbors = new Chunk[6];
            for (int i = 0; i < 6; i++) {
                var pos = position + Coord3.Directions[i];
                neighbors[i] = world.GetChunk(pos);
            }
        }

    }

}