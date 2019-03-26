using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;

namespace VoxelEngine.Blocks {

    public class VirusBlock : Block, IUpdateable {
        public VirusBlock(Block copy) : base(copy) { }

        int i = 0;
        void IUpdateable.OnTick() {
            if (i == 6) {
                chunk.world.OnTick -= ((IUpdateable)this).OnTick;
                return;
            }
            var block = chunk.GetBlock(position.WorldToBlock(chunk.worldPosition) + Coord3.Directions[i]);
            if (block != null && block.data.meshType == BlockMeshType.Cube && block.data.name == "grass" || block.data.name == "dirt") {
                chunk.SetBlock(data, position.WorldToBlock(chunk.worldPosition) + Coord3.Directions[i], Coord3.zero);
            }
            
            i++;
        }
    }

}