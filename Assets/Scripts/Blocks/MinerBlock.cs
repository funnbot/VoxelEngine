using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Internal;
using VoxelEngine.Serialization;

namespace VoxelEngine.Blocks {

    public class MinerBlock : RotatedBlock, ITickable {
        public MinerBlock() { }

        public bool mining;
        public Coord3 miningLocation;

        Coord3 position;
        ChunkSection chunk;

        public override void OnLoad(Coord3 pos, BlockData data, ChunkSection chunk) {
            position = pos;
            this.chunk = chunk;
        }

        void ITickable.OnTick() {
            if (!mining) return;
            chunk.blocks.PlaceBlock(miningLocation, null, true);

            miningLocation.z++;
            if (miningLocation.z > position.z + 2) {
                miningLocation.z = position.z - 2;
                miningLocation.x++;
            }
            if (miningLocation.x > position.x + 2) {
                miningLocation.x = position.x - 2;
                miningLocation.y--;
            }
            if (miningLocation.y <= -100) 
                mining = false;
        }

        public override void OnPlace() {
            miningLocation = position - new Coord3(2, 1, 2);
            mining = true;
        }

        public override void Serialize(System.IO.BinaryWriter writer) {
            base.Serialize(writer);
            miningLocation.Serialize(writer);
        }

        public override void Deserialize(System.IO.BinaryReader reader) {
            base.Deserialize(reader);
            miningLocation.Deserialize(reader);
        }
    }

}