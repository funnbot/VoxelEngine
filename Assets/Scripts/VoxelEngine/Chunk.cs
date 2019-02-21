using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.ProceduralGeneration;

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

        private Block[, , ] blocks;

        private MeshData blockMesh;
        private MeshData transMesh;
        private MeshData colliderMesh;

        public void Init(VoxelWorld world, Vector3Int position) {
            blocks = new Block[Chunk.Size, Chunk.Size, Chunk.Size];
            this.world = world;
            this.position = position;

            blockMesh = new MeshData();
            transMesh = new MeshData();
            colliderMesh = new MeshData();

            /*var blockMat = BlockRend.GetComponent<MeshRenderer>().sharedMaterial;
            BlockUVTileSize = 1f / ((float) blockMat.mainTexture.height / world.texturePixelResolution);
            var transMat = TransRend.GetComponent<MeshRenderer>().sharedMaterial;
            TransUVTileSize = 1f / ((float) transMat.mainTexture.height / world.texturePixelResolution);
            */

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
            Vector3Int.FloorToInt(block + position * Chunk.Size);

        public Vector3Int WorldToBlockPos(Vector3Int block) =>
            Vector3Int.FloorToInt(block - position * Chunk.Size);

        public bool InRange(int x, int y, int z) =>
            x >= 0 && x < Size && y >= 0 && y < Size && z >= 0 && z < Size;
        public bool InRange(Vector3Int p) => InRange(p.x, p.y, p.z);

        void HookEvents() {
            world.OnTick += OnTick;
        }

        public Vector2[] tst;

        void AddBlockToMesh(int x, int y, int z) {
            var pos = new Vector3Int(x, y, z);
            var block = blocks[x, y, z];

            if (block == null) {
                // Debug.LogError("Attemping to render chunk with null blocks.");
                return;
            }

            if (block.data.meshType == BlockMeshType.Cube) {
                for (int i = 0; i < 6; i++) {
                    int oppDirection = i % 2 == 0 ? i + 1 : i - 1;
                    var touching = DirOffsets[i] + pos;
                    var faceTouching = GetBlock(touching);

                    // if (!block.data.transparent && (faceTouching == null || !faceTouching.data.transparent)) continue;
                    if (!block.data.transparent && faceTouching != null && !faceTouching.data.transparent) continue;
                    // if (!block.data.transparent && faceTouching != null) continue;

                    int texIndex = TextureIndex(block.data.texIndices, i);
                    if (block.data.transparent) {
                        transMesh.AddQuad(i, pos, texIndex);
                        //var tiling = FaceUVs(block.data.FaceTiling, i, TransUVTileSize);
                        //transMesh.AddUVs(tiling);
                    } else {
                        blockMesh.AddQuad(i, pos, texIndex);
                        // var tiling = FaceUVs(block.data.FaceTiling, i, BlockUVTileSize);
                        //blockMesh.AddUVs(tiling);
                    }
                    colliderMesh.AddQuad(i, pos, texIndex);
                }
            } else if (block.data.meshType == BlockMeshType.Decal) {
                var texIndex = TextureIndex(block.data.texIndices, 0);
                transMesh.AddDecal(pos, texIndex);
                //var tiling = FaceUVs(block.data.FaceTiling, 0, TransUVTileSize);
                //for (int i = 0; i < 4; i++) transMesh.AddUVs(tiling);
            }
        }

        private int TextureIndex(int[] inds, int dir) {
            if (inds.Length == 1) return inds[0];
            else if (inds.Length == 3) return inds[Mathf.Min(dir, 2)];
            else return inds[dir];
        }

        /* Vector2[] FaceUVs(Vector2Int[] tiling, int dir, float UVTileSize) {
            var tile = tiling.Length == 1 ? tiling[0] : tiling[dir];
            return new Vector2[] {
                new Vector2(UVTileSize * tile.x, UVTileSize * tile.y + UVTileSize),
                new Vector2(UVTileSize * tile.x + UVTileSize, UVTileSize * tile.y + UVTileSize),
                new Vector2(UVTileSize * tile.x + UVTileSize, UVTileSize * tile.y),
                new Vector2(UVTileSize * tile.x, UVTileSize * tile.y)
            };
        } */

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