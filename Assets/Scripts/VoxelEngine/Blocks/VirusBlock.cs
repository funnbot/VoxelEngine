using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;

namespace VoxelEngine.Blocks {

    [MessagePackObject]
    public class VirusBlock : Block, INeighborUpdateable, IInterfaceable {
        public VirusBlock(Block copy) : base(copy) { }

        int i = 0;
        void OnTick() {
            if (i == 6) {
                return;
            }
            var block = chunk.GetBlock(position.WorldToBlock(chunk.worldPosition) + Coord3.Directions[i]);
            if (block != null && block.data.blockType == BlockType.Cube && (block.data.blockID == "grass" || block.data.blockID == "dirt")) {
                chunk.SetBlock(data, position.WorldToBlock(chunk.worldPosition) + Coord3.Directions[i], Coord3.zero);
            }
            i++;
        }

        void INeighborUpdateable.OnNeighborUpdate(Block block) {

        }

        void IInterfaceable.BuildGUI() {

        }

        void IInterfaceable.OpenGUI() {

        }

        void IInterfaceable.CloseGUI() {

        }
    }

}