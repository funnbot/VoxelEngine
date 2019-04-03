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
        public Block[][][] blocks;

        static byte stoneID = 0;

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

        public void Serialize(BinaryWriter writer) {
            if (chunk.IsAllAir) {
                writer.Write((byte) ReservedBytes.AllAir);
                return;
            }

            if (chunk.IsAllStone) {
                writer.Write((byte) ReservedBytes.AllStone);
                return;
            }

            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        SerializeBlock(writer, blocks[x][y][z]);
                    }
                }
            }
        }

        public void Deserialize(BinaryReader reader) {
            var id = reader.ReadByte();

            if (id == (byte) ReservedBytes.AllAir) return;

            if (id == (byte) ReservedBytes.AllStone) {
                FillWithBlock(stoneID);
                return;
            }

            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        DeserializeBlock(reader, id, ref blocks[x][y][z]);
                        id = reader.ReadByte();
                    }
                }
            }
        }

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

        void DeserializeBlock(BinaryReader reader, byte id, ref Block block) {
            if (id == (byte) ReservedBytes.Air) return;

            var data = ResourceStore.Blocks[id];
            if (data.IsCustomType) {
                block = Block.Convert(id, data.dataType);
                block.Deserialize(reader);
            } else block = new Block { id = id };
        }

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
                    chunk.QueueUpdate();
                    chunk.UpdateNeighbors(localPos);
                }
                return chunk;
            } else return chunk.world.PlaceBlock(ToWorldSpace(localPos), data);
        }
        public ChunkSection PlaceBlock(Coord3 localPos, BlockData data, out Block block, bool updateChunk = true) {
            if (InRange(localPos)) {
                DestroyBlock(blocks[localPos.x][localPos.y][localPos.z]);
                CreateBlock(localPos, data, ref blocks[localPos.x][localPos.y][localPos.z]);

                chunk.SetDirty();
                if (updateChunk) {
                    chunk.QueueUpdate();
                    chunk.UpdateNeighbors(localPos);
                }
                block = blocks[localPos.x][localPos.y][localPos.z];
                return chunk;
            } else return chunk.world.PlaceBlock(ToWorldSpace(localPos), data, out block, updateChunk);
        }

        public void FillWithBlock(byte id) {
            for (int x = 0; x < ChunkSection.Size; x++) {
                for (int y = 0; y < ChunkSection.Size; y++) {
                    for (int z = 0; z < ChunkSection.Size; z++) {
                        blocks[x][y][z] = new Block { id = id };
                    }
                }
            }
        }

        public void GetCustomBlock<T>(Block block, System.Action<T> predicate, bool updateChunk = false) where T : Block {
            if (block == null) return;
            var blk = this as T;
            if (blk != null) {
                predicate(blk);
                chunk.SetDirty();
                if (updateChunk) {
                    chunk.QueueUpdate();
                }
            }
        }
        public void GetCustomBlock<T>(Coord3 pos, System.Action<T> predicate, bool updateChunk = false) where T : Block =>
            GetCustomBlock<T>(GetBlock(pos), predicate, updateChunk);

        public Block GetBlock(Coord3 pos) {
            if (!chunk.built) return null;
            if (InRange(pos)) return blocks[pos.x][pos.y][pos.z];
            return chunk.world.GetBlock(ToWorldSpace(pos));
        }

        public Block GetBlockRaw(Coord3 pos) => blocks[pos.x][pos.y][pos.z];
        public Block GetBlockRaw(int x, int y, int z) => blocks[x][y][z];

        public Block[][][] GetBlocksRaw() => blocks;

        public void UpdateBlockNeighbors(Coord3 pos) {
            foreach (var dir in Coord3.Directions) {
                var block = GetBlock(pos + dir);
                block?.OnNeighborUpdate();
            }
        }

        public void LoadBlock(Coord3 pos, BlockData data, Block block) {
            block.OnLoad(pos, data, chunk);
            var tickable = block as ITickable;
            if (tickable != null) chunk.world.OnTick += tickable.OnTick;
        }

        public void UnloadBlock(Block block) {
            var tickable = block as ITickable;
            if (tickable != null) chunk.world.OnTick -= tickable.OnTick;
            block.OnUnload();
        }

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