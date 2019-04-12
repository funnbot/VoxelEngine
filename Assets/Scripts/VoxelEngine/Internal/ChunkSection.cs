using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Serialization;
using VoxelEngine.Utilities;
using BinaryWriter = System.IO.BinaryWriter;
using BinaryReader = System.IO.BinaryReader;

namespace VoxelEngine.Internal {

    public class ChunkSection : MonoBehaviour {
        /// Square size of all chunk sections
        public static readonly int Size = 16;

        public Transform Blocks;
        public MeshFilter BlockRend;
        public MeshCollider BlockCollider;
        public MeshCollider BlockTrigger;

        /// World this chunk is in
        public VoxelWorld world { get; set; }
        /// Position of this section
        public Coord3 position { get; set; }
        /// World position of this section
        public Coord3 worldPosition { get; set; }

        /// Collection of blocks in this section
        public BlockManager blocks;

        /// Get if the parent chunk has been built
        public bool IsBuilt { get => parent.IsBuilt; }

        /// When loaded if this chunk only contains air
        public bool IsAllAir { get; set; }
        /// When loaded if this chunk only contains stone
        public bool IsAllStone { get; set; }

        /// Queue a chunk to rerender
        private bool renderUpdate;

        private Chunk parent;

        private MeshBuilder blockMesh;
        private MeshBuilder colliderMesh;
        private MeshBuilder triggerMesh;

        /// Create the section to be stored in the pool, run once
        public void Create(Chunk parent, VoxelWorld world) {
            this.world = world;
            this.parent = parent;

            transform.parent = parent.transform;

            blockMesh = new MeshBuilder();
            colliderMesh = new MeshBuilder();
            triggerMesh = new MeshBuilder();

            blocks = new BlockManager(this);
        }

        /// Setup the chunk after being pulled from the pool
        public void Setup(Coord3 position) {
            IsAllAir = true;
            IsAllStone = true;
            renderUpdate = false;

            this.position = position;
            worldPosition = position * Size;
            transform.localPosition = Coord3.up * worldPosition;
            name = "ChunkSection " + position;

            world.OnTick += OnTick;
        }

        /// Cleanup the chunk before being released back into the pool
        public void CleanUp() {
            BlockRend.sharedMesh = null;
            BlockCollider.sharedMesh = null;
            BlockTrigger.sharedMesh = null;

            world.OnTick -= OnTick;
        }

        /// Set the parent to dirty because a change was made
        public void SetDirty() {
            parent.IsDirty = true;
        }

        /// Set renderUpdate to true to try and render this chunk
        public void Render() {
            renderUpdate = true;
        }

        /// Update the neighbors around this chunk
        public void UpdateNeighbors(Coord3 changed) {
            if (changed.InRange(1, ChunkSection.Size - 1)) return;
            for (int i = 0; i < 6; i++) {
                var pos = changed + Coord3.Directions[i];
                if (!pos.InRange(0, ChunkSection.Size)) {
                    var neighbor = world.chunks.GetSection(position + Coord3.Directions[i]);
                    if (neighbor == null || !neighbor.IsBuilt) continue;
                    neighbor.Render();
                }
            }
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

        private void OnTick() {
            if (renderUpdate) TryRender();
        }

        /// Attempt a render, only if it hasnt surpassed the max amount of renders per tick
        private void TryRender() {
            if (!IsBuilt || IsAllAir) return;
            if (world.chunkRenders > VoxelWorld.MaxRendersPerTick) return;

            GenerateMesh();
            ApplyMesh();
            world.chunkRenders++;
            renderUpdate = false;
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
                                int index = RotationUtil.IndexRotateAround(i, ref rotatedBlock.rotation);
                                texIndex = data.textureIndices[index];
                                //texIndex = i;
                                faceRot = RotationUtil.FaceRotate(i, ref rotatedBlock.rotation);
                            }
                        }
                        //Debug.Log(texIndex);
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
                    if (data.collision)
                        colliderMesh.AddBoundingBox(pos, data.boundingSize);
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
                // If chunk was just loaded but not built, then dont render
                if (!chunk.IsBuilt) return true;
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