using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;

namespace VoxelEngine.Blocks {

    [MessagePackObject]
    public class VirusBlock : Block, IInterfaceable {
        public VirusBlock(Block copy) : base(copy) { }
        public VirusBlock() { }

        int i = 0;
        void OnTick() {
            if (i == 6) {
                return;
            }
            var block = chunk.GetBlock(position.WorldToBlock(chunk.worldPosition) + Coord3.Directions[i]);
            if (block != null && block.data.blockType == BlockType.Cube && (block.data.blockId == "grass" || block.data.blockId == "dirt")) {
                chunk.SetBlock(new Block(data), position.WorldToBlock(chunk.worldPosition) + Coord3.Directions[i]);
            }
            i++;
        }

        public override void OnNeighborUpdate(Block block) {
            
        }

        void IInterfaceable.BuildGUI() {

        }

        void IInterfaceable.OpenGUI() {

        }

        void IInterfaceable.CloseGUI() {

        }
    }

}