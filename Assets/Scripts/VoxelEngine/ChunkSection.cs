using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Serialization;
using VoxelEngine.Interfaces;

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

        public void InitializeBlock(ref Block block, Coord3 position) {
            if (block.data.dataType.Length > 0) {
                block = block.ConvertTo(block.data.dataType);
            }

            RegisterBlock(ref block, position);

            if (block.data.dataType.Length > 0) {
                IPlaceHandler placeHandler = block as IPlaceHandler;
                if (placeHandler != null) {
                    placeHandler.OnPlace();
                }
            }
        }

        public void DestroyBlock(Block block) {
            if (block.data.dataType.Length > 0) {
                IBreakHandler breakHandler = block as IBreakHandler;
                if (breakHandler != null) {
                    breakHandler.OnBreak();
                }
            }

            UnregisterBlock(block);
        }

        public void RegisterBlock(ref Block block, Coord3 position) {
            block.position = position;
            block.chunk = this;

            if (block.data.blockType == BlockType.Custom) {
                var go = Instantiate(block.data.prefab, position, Quaternion.Euler(block.rotation * 90), Blocks);
                go.name = block.data.blockID + " " + block.position;

                StandaloneBlock standalone = block as StandaloneBlock;
                if (standalone == null) standalone = new StandaloneBlock(block);

                standalone.gameObject = go;
                block = standalone;
            }

            if (block.data.dataType.Length > 0) {
                IUpdateable inter = block as IUpdateable;
                if (inter != null) {
                    world.OnTick += inter.OnTick;
                }
            }
        }

        public void UnregisterBlock(Block block) {
            if (block.data.dataType.Length > 0) {
                IUpdateable inter = block as IUpdateable;
                if (inter != null) {
                    world.OnTick -= inter.OnTick;
                }
            }

            if (block.data.blockType == BlockType.Custom) {
                StandaloneBlock standalone = block as StandaloneBlock;
                if (standalone != null && standalone.gameObject != null) {
                    Destroy(standalone.gameObject, 0.1f);
                }
            }
        }

        public void Create(Chunk parent, VoxelWorld world) {
            this.world = world;
            this.parent = parent;

            transform.parent = parent.transform;

            blockMesh = new MeshData();
            colliderMesh = new MeshData();
            triggerMesh = new MeshData();

            blocks = new Block[Size][][];
            for (int x = 0; x < Size; x++) {
                blocks[x] = new Block[Size][];
                for (int y = 0; y < Size; y++) {
                    blocks[x][y] = new Block[Size];
                }
            }
        }

        public void Setup(Coord3 position) {
            update = false;

            this.position = position;
            worldPosition = position * Size;
            transform.localPosition = Coord3.up * worldPosition;
            name = "ChunkSection " + position;

            world.OnTick += OnTick;
        }

        public void CleanUp() {
            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    for (int z = 0; z < Size; z++) {
                        var block = blocks[x][y][z];
                        if (block == null) continue;
                        UnregisterBlock(block);
                        blocks[x][y][z] = null;
                    }
                }
            }

            world.OnTick -= OnTick;

            blockMesh.Clear();
            colliderMesh.Clear();
            triggerMesh.Clear();

            BlockRend.sharedMesh = null;
            BlockCollider.sharedMesh = null;
            BlockTrigger.sharedMesh = null;
        }

        public void Serialize(SerialChunk serial, int w) {
            serial.blocks[w] = blocks;
        }

        public void Deserialize(SerialChunk serial, int w) {
            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    for (int z = 0; z < Size; z++) {
                        var block = serial.blocks[w][x][y][z];
                        blocks[x][y][z] = block;
                        // Air will be null now
                        if (block == null) continue;
                        block.data = ResourceStore.Blocks[block.id];

                        RegisterBlock(ref block, new Coord3(x, y, z).BlockToWorld(worldPosition));
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

        public Block GetBlock(Coord3 pos) {
            if (!parent.built) return null;
            if (pos.InRange(0, ChunkSection.Size)) return blocks[pos.x][pos.y][pos.z];
            else return world.GetBlock(pos.BlockToWorld(worldPosition));
        }

        public void SetBlock(Block block, Coord3 pos, bool updateChunk = true) {
            if (block?.data.blockType == BlockType.Entity) {
                world.SpawnEntity(block, pos, this);
                return;
            }

            if (pos.InRange(0, ChunkSection.Size)) {
                var oldBlock = blocks[pos.x][pos.y][pos.z];

                if (oldBlock != null) DestroyBlock(oldBlock);
                if (block != null) InitializeBlock(ref block, pos.BlockToWorld(worldPosition));

                blocks[pos.x][pos.y][pos.z] = block;

                parent.isDirty = true;
                if (updateChunk) {
                    QueueUpdate();
                    UpdateNeighbors(pos);
                }
            } else world.SetBlock(block, pos.BlockToWorld(worldPosition), updateChunk);
        }
        
        public void SetBlock(BlockData blockData, Coord3 pos, Coord3 rot, bool updateChunk = true) {
            var block = blockData != null ? new Block(blockData, rot) : null;
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

        private void AddBlockToMesh(int x, int y, int z) {
            var pos = new Coord3(x, y, z);
            var block = blocks[x][y][z];

            if (block == null) return;

            switch (block.data.blockType) {
                case BlockType.Cube:
                    for (int i = 0; i < 6; i++) {
                        if (CullFace(block, pos, i)) continue;

                        var rot = Rotate(i, block.rotation);
                        int texIndex = block.data.textureIndices[rot.index];

                        blockMesh.AddCubeFace(i, pos, rot.face, texIndex, (int) block.data.subMesh);

                        if (block.data.collision)
                            colliderMesh.AddCubeFace(i, pos);
                        else triggerMesh.AddCubeFace(i, pos);
                    }
                    break;
                case BlockType.DecalCross:
                    var tex = block.data.textureIndices[0];
                    blockMesh.AddDecalCross(pos, tex, (int) block.data.subMesh);

                    if (block.data.collision)
                        colliderMesh.AddBoundingBox(pos, block.data.boundingSize);
                    else triggerMesh.AddBoundingBox(pos, block.data.boundingSize);
                    break;
                case BlockType.Custom:
                    if (block.data.collision)
                        colliderMesh.AddBoundingBox(pos, block.data.boundingSize);
                    break;
            }
        }

        private bool CullFace(Block block, Coord3 pos, int dir) {
            // If this face is transparent, always render
            if (block.data.subMesh == SubMesh.Transparent) return false;

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
                adjacent = chunk.GetBlock(worldAdjPos.WorldToBlock(chunk.worldPosition));
            } else {
                adjacent = GetBlock(adjPos);
            }
            // If block is not null/air, and block isnt transparent, don't render;
            return adjacent != null && adjacent.data.subMesh != SubMesh.Transparent;
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