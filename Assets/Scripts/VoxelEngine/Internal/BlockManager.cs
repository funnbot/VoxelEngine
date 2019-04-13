using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Serialization;
using BinaryWriter = System.IO.BinaryWriter;
using BinaryReader = System.IO.BinaryReader;

namespace VoxelEngine.Internal {

    public class BlockManager {
        private ChunkSection chunk;
        private Block[][][] blocks;

        static byte stoneID = 0;

        /// Create a new block manager
        public BlockManager(ChunkSection chunk) {
            stoneID = ResourceStore.Blocks.GetId("stone");

            this.chunk = chunk;
            this.blocks = new Block[ChunkSection.Size][][];
            for (int x = 0; x < ChunkSection.Size; x++) {
                this.blocks[x] = new Block[ChunkSection.Size][];
                for (int y = 0; y < ChunkSection.Size; y++) {
                    this.blocks[x][y] = new Block[ChunkSection.Size];
                }
            }
        }

        /// Serializes the blocks in this manager
        public void Serialize(BinaryWriter writer) {
            // if (chunk.IsAllAir) {
            //     writer.Write((byte) ReservedBytes.AllAir);
            //     return;
            // }

            // if (chunk.IsAllStone) {
            //     writer.Write((byte) ReservedBytes.AllStone);
            //     return;
            // }

            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        SerializeBlock(writer, blocks[x][y][z]);
                    }
                }
            }
        }

        /// Deserializes the blocks in this manager
        public void Deserialize(BinaryReader reader) {
            // var id = reader.ReadByte();

            // if (id == (byte) ReservedBytes.AllAir) return;

            // if (id == (byte) ReservedBytes.AllStone) {
            //     FillWithBlock(stoneID);
            //     return;
            // }

            // DeserializeBlock(reader, id, ref blocks[0][0][0]);

            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        var id = reader.ReadByte();
                        DeserializeBlock(reader, id, ref blocks[x][y][z]);
                    }
                }
            }
        }

        /// Serialize a specific block
        void SerializeBlock(BinaryWriter writer, Block block) {
            if (block == null) {
                writer.Write((byte) ReservedBytes.Air);
                return;
            }

            var data = ResourceStore.Blocks[block.id];
            writer.Write(block.id);

            if (data.IsCustomType) {
                block.Serialize(writer);
            }
        }

        /// Deserialize a specific block
        void DeserializeBlock(BinaryReader reader, byte id, ref Block block) {
            if (id == (byte) ReservedBytes.Air) return;
            BlockData data;

            data = ResourceStore.Blocks[id];

            if (data.IsCustomType) {
                block = Block.Convert(id, data.dataType);
                block.Deserialize(reader);
            } else block = new Block { id = id };
        }

        /// Place a block data in a position
        public ChunkSection PlaceBlock(Coord3 localPos, BlockData data, bool updateChunk = true) {
            if (InRange(localPos)) {
                DestroyBlock(blocks[localPos.x][localPos.y][localPos.z]);
                CreateBlock(localPos, data, ref blocks[localPos.x][localPos.y][localPos.z]);

                if (data != null) {
                    chunk.IsAllAir = false;
                    if (data.id != stoneID) chunk.IsAllStone = false;
                }

                chunk.SetDirty();
                if (updateChunk) {
                    chunk.Render();
                    chunk.UpdateNeighbors(localPos);
                }
                return chunk;
            } else return chunk.world.PlaceBlock(ToWorldSpace(localPos), data, updateChunk);
        }
        /// Place a block data in a position and get out the created block
        public ChunkSection PlaceBlock(Coord3 localPos, BlockData data, out Block block, bool updateChunk = true) {
            if (InRange(localPos)) {
                DestroyBlock(blocks[localPos.x][localPos.y][localPos.z]);
                CreateBlock(localPos, data, ref blocks[localPos.x][localPos.y][localPos.z]);

                if (data != null) {
                    chunk.IsAllAir = false;
                    if (data.id != stoneID) chunk.IsAllStone = false;
                }

                chunk.SetDirty();
                if (updateChunk) {
                    chunk.Render();
                    chunk.UpdateNeighbors(localPos);
                }
                block = blocks[localPos.x][localPos.y][localPos.z];
                return chunk;
            } else return chunk.world.PlaceBlock(ToWorldSpace(localPos), data, out block, updateChunk);
        }

        /// Fill the entire chunk with a block id
        public void FillWithBlock(byte id) {
            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        blocks[x][y][z] = new Block { id = id };
                    }
                }
            }
        }

        /// Run a custom action on a block
        public void GetCustomBlock<T>(Block block, System.Action<T> predicate, bool updateChunk = false) where T : Block {
            if (block == null) return;
            if (block is T b) {
                predicate(b);
                chunk.SetDirty();
                if (updateChunk) chunk.Render();
            }
        }
        public void GetCustomBlock<T>(Coord3 pos, System.Action<T> predicate, bool updateChunk = false) where T : Block =>
            GetCustomBlock<T>(GetBlock(pos), predicate, updateChunk);

        /// Get a block from this manager
        public Block GetBlock(Coord3 pos) {
            if (!chunk.IsBuilt) return null;
            if (InRange(pos)) return blocks[pos.x][pos.y][pos.z];
            return chunk.world.GetBlock(ToWorldSpace(pos));
        }

        /// Directly pull from the blocks array without a range check
        public Block GetBlockRaw(Coord3 pos) => blocks[pos.x][pos.y][pos.z];
        public Block GetBlockRaw(int x, int y, int z) => blocks[x][y][z];

        /// Get a reference to the blocks array
        public Block[][][] GetBlocksRaw() => blocks;

        /// Run the update event on all neighbors of a block position
        public void UpdateBlockNeighbors(Coord3 pos) {
            foreach (var dir in Coord3.Directions) {
                var block = GetBlock(pos + dir);
                block?.OnNeighborUpdate();
            }
        }

        /// Loads custom actions of a block
        public void LoadBlock(Coord3 pos, BlockData data, Block block) {
            block.OnLoad(pos, data, chunk);
            if (block is ITickable tickable)
                chunk.world.OnTick -= tickable.OnTick;
        }

        /// Unloads the custom actions of a block
        public void UnloadBlock(Block block) {
            if (block is ITickable tickable)
                chunk.world.OnTick -= tickable.OnTick;
            block.OnUnload();
        }

        /// Apples load to all blocks
        public void LoadAll() {
            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        var block = blocks[x][y][z];
                        if (block == null) continue;
                        var pos = new Coord3(x, y, z);
                        var data = ResourceStore.Blocks[block.id];
                        LoadBlock(pos, data, block);
                    }
                }
            }
        }

        /// Unload all blocks and prepare for deletion
        public void UnloadAll() {
            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        var block = blocks[x][y][z];
                        if (block == null) continue;
                        UnloadBlock(block);
                        blocks[x][y][z] = null;
                    }
                }
            }
        }

        /// When a new block it placed, setup custom actions
        void CreateBlock(Coord3 pos, BlockData data, ref Block block) {
            if (data == null) {
                block = null;
                return;
            }
            if (!data.IsCustomType && !data.IsStandalone) {
                block = new Block { id = (byte) data.id };
                return;
            }

            var type = data.IsCustomType ? data.dataType : BlockDataType.StandaloneBlock;
            block = Block.Convert((byte) data.id, type);

            LoadBlock(pos, data, block);
            block.OnPlace();
        }

        /// When a block is destroyed, remove custom actions
        void DestroyBlock(Block block) {
            if (block == null) return;
            var data = ResourceStore.Blocks.GetData(block.id);
            if (data.IsCustomType || data.IsStandalone) {
                block.OnBreak();
                UnloadBlock(block);
            }
        }

        Coord3 ToWorldSpace(Coord3 blockPos) => blockPos + chunk.worldPosition;
        Coord3 ToBlockSpace(Coord3 worldPos) => worldPos - chunk.worldPosition;
        bool InRange(Coord3 blockPos) => blockPos.InRange(0, ChunkSection.Size);
        Coord3 TransformChunk(Coord3 blockPos, Coord3 newWorldPosition) =>
            blockPos + chunk.worldPosition - newWorldPosition;
    }

}