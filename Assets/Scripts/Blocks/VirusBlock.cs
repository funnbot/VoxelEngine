using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Internal;

namespace VoxelEngine.Blocks {

    public class VirusBlock : CustomBlock {
        public VirusBlock(byte id) : base(id) { }

        int i = 0;

        public override void OnNeighborUpdate() {
            if (i == 6) {
                return;
            }
            var pos = position + Coord3.Directions[i];
            var block = chunk.blocks.GetBlock(pos);
            if (block != null && (block.id == ResourceStore.Blocks.GetId("grass") || block.id == ResourceStore.Blocks.GetId("dirt"))) {
                Block placed;
                chunk.blocks.PlaceBlock(pos, data, out placed, true);
            }
            i++;
        }
    }

}