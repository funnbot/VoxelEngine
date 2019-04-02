using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Serialization;

namespace VoxelEngine.Internal {

    public class ChunkSection : MonoBehaviour {
        public static readonly int Size = 16;

        public Transform Blocks;
        public MeshFilter BlockRend;
        public MeshCollider BlockCollider;
        public MeshCollider BlockTrigger;

        public VoxelWorld world;
        public Coord3 position;
        public Coord3 worldPosition;

        public BlockManager blocks;

        public MeshData blockMesh;
        public MeshData colliderMesh;
        public MeshData triggerMesh;

        public bool built { get => parent.built; }

        private Chunk parent;

        private bool update;

        public void Create(Chunk parent, VoxelWorld world) {
            this.world = world;
            this.parent = parent;

            transform.parent = parent.transform;

            blockMesh = new MeshData();
            colliderMesh = new MeshData();
            triggerMesh = new MeshData();

            blocks = new BlockManager(this);
        }

        public void Setup(Coord3 position) {
            update = false;

            this.position = position;
            worldPosition = position * Size;
            transform.localPosition = Coord3.up * worldPosition;
            name = "ChunkSection " + position;

            world.chunks.OnChunkUpdate += OnChunkUpdate;
        }

        public void CleanUp() {
            blocks.UnloadAll();

            blockMesh.Clear();
            colliderMesh.Clear();
            triggerMesh.Clear();

            BlockRend.sharedMesh = null;
            BlockCollider.sharedMesh = null;
            BlockTrigger.sharedMesh = null;

            world.chunks.OnChunkUpdate -= OnChunkUpdate;
        }

        public void Serialize(SerialChunk serial, int w) {
            serial.blocks[w] = blocks.GetBlocksRaw();
        }

        public void Deserialize(SerialChunk serial, int w) {
            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    for (int z = 0; z < Size; z++) {
                        var block = serial.blocks[w][x][y][z];
                        blocks.SetBlockRaw(x, y, z, block);

                        if (block == null) continue;
                        var data = ResourceStore.Blocks[block.id];

                        blocks.LoadBlock(new Coord3(x, y, z), data, block);
                    }
                }
            }
        }

        public void SetDirty() {
            parent.isDirty = true;
        }

        void OnChunkUpdate() {
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
                        AddBlockToMesh(x, y, z);
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

        private void AddBlockToMesh(int x, int y, int z) {
            var pos = new Coord3(x, y, z);
            var block = blocks.GetBlockRaw(x, y, z);
            if (block == null) return;

            var data = ResourceStore.Blocks[block.id];
            switch (data.blockType) {
                case BlockType.Cube:
                    for (int i = 0; i < 6; i++) {
                        if (CullFace(data, pos, i)) continue;

                        int faceRot = 0;
                        int texIndex = data.textureIndices[i];

                        if (data.IsCustomType) {
                            var rotatedBlock = block as RotatedBlock;
                            if (rotatedBlock != null) {
                                var rot = Rotate(i, rotatedBlock.rotation);
                                faceRot = rot.face;
                                texIndex = data.textureIndices[rot.index];
                            }
                        }

                        blockMesh.AddCubeFace(i, pos, faceRot, texIndex, (int) data.subMesh);

                        if (data.collision)
                            colliderMesh.AddCubeFace(i, pos);
                        else triggerMesh.AddCubeFace(i, pos);
                    }
                    break;
                case BlockType.DecalCross:
                    var tex = data.textureIndices[0];
                    blockMesh.AddDecalCross(pos, tex, (int) data.subMesh);

                    if (data.collision)
                        colliderMesh.AddBoundingBox(pos, data.boundingSize);
                    else triggerMesh.AddBoundingBox(pos, data.boundingSize);
                    break;
                case BlockType.Custom:
                    //if (data.collision)
                    //    colliderMesh.AddBoundingBox(pos, data.boundingSize);
                    break;
            }
        }

        private bool CullFace(BlockData data, Coord3 pos, int dir) {
            // If this face is transparent, always render
            if (data.subMesh == SubMesh.Transparent) return false;

            var adjPos = pos + Coord3.Directions[dir];
            Block adjacent;

            if (!adjPos.InRange(0, Size)) {
                var worldAdjPos = adjPos.BlockToWorld(worldPosition);
                var chunkPos = worldAdjPos.WorldToChunk();
                var chunk = world.chunks.GetSection(chunkPos);
                // if the neighbor chunk isnt there then render, 
                if (chunk == null) return false;
                // If chunk was just loaded but not built, then don't render
                if (!chunk.parent.built) return true;
                // get adjacent
                adjacent = chunk.blocks.GetBlock(worldAdjPos.WorldToBlock(chunk.worldPosition));
            } else {
                adjacent = blocks.GetBlock(adjPos);
            }
            // If block is not null/air, and block isnt transparent, don't render;
            return adjacent != null && ResourceStore.Blocks[adjacent.id].subMesh != SubMesh.Transparent;
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
    }

}