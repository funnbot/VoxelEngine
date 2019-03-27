using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Serialization;

namespace VoxelEngine {

    [System.Serializable]
    public class ChunkSection : MonoBehaviour {
        public static readonly int Size = 16;

        public Transform Blocks;
        public MeshFilter BlockRend;
        public MeshCollider BlockCollider;
        public MeshCollider BlockTrigger;

        public VoxelWorld world;
        public Coord3 position;
        public Coord3 worldPosition;

        public Block[][][] blocks;

        public MeshData blockMesh;
        public MeshData colliderMesh;
        public MeshData triggerMesh;

        private Chunk parent;
        private ChunkSection[] neighbors;

        private bool update;

        public void Create(Chunk parent, VoxelWorld world) {
            this.world = world;
            this.parent = parent;

            transform.parent = parent.transform;

            blockMesh = new MeshData();
            colliderMesh = new MeshData();
            triggerMesh = new MeshData();
        }

        public void Init(Coord3 position) {
            blocks = new Block[Size][][];
            for (int x = 0; x < Size; x++) {
                blocks[x] = new Block[Size][];
                for (int y = 0; y < Size; y++) {
                    blocks[x][y] = new Block[Size];
                }
            }

            update = false;

            this.position = position;
            worldPosition = position * Size;
            transform.localPosition = Coord3.up * worldPosition;
            name = "ChunkSection " + position;

            world.OnTick += OnTick;
        }

        public void CleanUp() {
            world.OnTick -= OnTick;
            blocks = null;

            blockMesh.Clear();
            colliderMesh.Clear();
            triggerMesh.Clear();

            BlockRend.sharedMesh = null;
            BlockCollider.sharedMesh = null;
            BlockTrigger.sharedMesh = null;

            foreach (GameObject child in Blocks) {
                Destroy(child);
            }
        }

        public void Serialize(SerialChunk serial, int w) {
            serial.blocks[w] = blocks;
        }

        public void Deserialize(SerialChunk serial, int w) {
            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    for (int z = 0; z < Size; z++) {
                        var block = serial.blocks[w][x][y][z];

                        block.data = ResourceStore.Blocks[block.id];

                        var pos = new Coord3(x, y, z).BlockToWorld(worldPosition);
                        blocks[x][y][z] = world.RegisterBlock(block, pos, this);
                    }
                }
            }
        }

        void OnTick() {
            if (update) TryRender();
        }

        public void TryRender() {
            if (world.chunkRenders >= VoxelWorld.MaxRendersPerTick) return;

            GenerateMesh();
            ApplyMesh();
            update = false;
            world.chunkRenders++;
        }

        public void GenerateMesh() {
            blockMesh.Clear();
            colliderMesh.Clear();
            triggerMesh.Clear();

            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    for (int z = 0; z < Size; z++) {
                        if (!AddBlockToMesh(x, y, z)) return;
                    }
                }
            }
        }

        public void ApplyMesh() {
            BlockRend.sharedMesh = blockMesh.ToMesh();
            BlockCollider.sharedMesh = colliderMesh.ToColMesh();
            BlockTrigger.sharedMesh = triggerMesh.ToColMesh();
        }

        public void QueueUpdate() {
            this.update = true;
        }

        public Block GetBlock(Coord3 pos) {
            if (!parent.built) return null;
            if (pos.InRange(0, ChunkSection.Size)) return blocks?[pos.x][pos.y][pos.z];
            else return world.GetBlock(pos.BlockToWorld(worldPosition));
        }

        public void SetBlock(Block block, Coord3 pos, bool updateChunk = true) {
            if (block.data.blockType == BlockType.Entity) {
                world.SpawnEntity(block, pos, this);
                return;
            }

            if (pos.InRange(0, ChunkSection.Size)) {
                world.UnregisterBlock(blocks[pos.x][pos.y][pos.z]);
                blocks[pos.x][pos.y][pos.z] =
                    world.RegisterBlock(block, pos.BlockToWorld(worldPosition), this);

                if (updateChunk) {
                    QueueUpdate();
                    UpdateNeighbors(pos);
                }
            } else world.SetBlock(block, pos.BlockToWorld(worldPosition), updateChunk);
        }
        public void SetBlock(BlockData blockData, Coord3 pos, Coord3 rot, bool updateChunk = true) {
            var block = new Block(blockData, rot);
            SetBlock(block, pos, updateChunk);
        }

        public void UpdateNeighbors(Coord3 changed) {
            if (changed.InRange(1, ChunkSection.Size - 1)) return;
            for (int i = 0; i < 6; i++) {
                var pos = changed + Coord3.Directions[i];
                if (!pos.InRange(0, ChunkSection.Size)) {
                    var neighbor = world.chunks.GetSection(position + Coord3.Directions[i]);
                    if (neighbor == null || !neighbor.parent.built) continue;
                    neighbor.QueueUpdate();
                }
            }
        }

        private bool AddBlockToMesh(int x, int y, int z) {
            var pos = new Coord3(x, y, z);
            var block = blocks[x][y][z];

            if (block == null) return false;

            if (block.data.blockType == BlockType.Cube) {
                for (int i = 0; i < 6; i++) {
                    if (CullFace(block, pos, i)) continue;

                    var rot = Rotate(i, block.rotation);
                    int texIndex = block.data.textureIndices[rot.index];

                    blockMesh.AddCubeFace(i, pos, rot.face, texIndex, (int) block.data.subMesh);

                    if (block.data.collision) {
                        colliderMesh.AddCubeFace(i, pos);
                    } else {
                        triggerMesh.AddCubeFace(i, pos);
                    }
                }
            } else if (block.data.blockType == BlockType.DecalCross) {
                var texIndex = block.data.textureIndices[0];
                blockMesh.AddDecalCross(pos, texIndex, (int) block.data.subMesh);
                if (block.data.collision) {
                    colliderMesh.AddBoundingBox(pos, block.data.boundingSize);
                } else {
                    triggerMesh.AddBoundingBox(pos, block.data.boundingSize);
                }
            }

            return true;
        }

        private bool CullFace(Block block, Coord3 pos, int dir) {
            if (block.data.subMesh == SubMesh.Transparent) return false;
            var adjacent = GetBlock(Coord3.Directions[dir] + pos);
            return adjacent == null || adjacent.data.subMesh != SubMesh.Transparent;
        }

        private static readonly int[] zRot = { BlockFace.top, BlockFace.left, BlockFace.bottom, BlockFace.right },
            yRot = { BlockFace.front, BlockFace.right, BlockFace.back, BlockFace.left },
            xRot = { BlockFace.front, BlockFace.top, BlockFace.back, BlockFace.bottom };

        private static readonly int[][] zFace = { new [] { 3, 1, 3, 3, 3, 3 }, new [] { 2, 2, 2, 2, 2, 2, }, new [] { 1, 3, 1, 1, 1, 1 } },
            yFace = { new [] { 0, 0, 3, 1, 0, 0 }, new [] { 0, 0, 2, 2, 0, 0 }, new [] { 0, 0, 1, 3, 0, 0, } },
            xFace = { new [] { 0, 2, 0, 2, 1, 3 }, new [] { 2, 2, 0, 2, 2, 2 }, new [] { 0, 2, 2, 0, 3, 1 } };

        private static(int index, int face) Rotate(int dir, Coord3 rot) {
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
    }

}