using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Internal;

namespace VoxelEngine.Blocks {

    public class VirusBlock : Block {
        public VirusBlock() { }

        int i = 0;

        ChunkSection chunk;
        Coord3 position;
        BlockData data;

        public override void OnNeighborUpdate() {
            if (i == 6) {
                return;
            }
            var pos = position + Coord3.Directions[i];
            var block = chunk.blocks.GetBlock(pos);
            if (block != null && (block.id == ResourceStore.Blocks.GetId("grass") || block.id == ResourceStore.Blocks.GetId("dirt"))) {
                Block placed;
                chunk.blocks.PlaceBlock(pos, data, out placed, true);
                placed.OnNeighborUpdate();
            }
            i++;
        }


        public override void OnLoad(Coord3 pos, BlockData data, ChunkSection chunk) {
            position = pos;
            this.data = data;
            this.chunk = chunk;
        }
    }

}