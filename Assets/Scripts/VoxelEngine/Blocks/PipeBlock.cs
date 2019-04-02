using UnityEngine;
using VoxelEngine.Interfaces;

namespace VoxelEngine.Blocks {

    public class PipeBlock : StandaloneBlock {
        PipeObject obj;

        [Ceras.Include]
        public PipeType type;
        [Ceras.Include]
        public Coord3 rotation;

        public PipeBlock(Block block) : base(block) { }
        public PipeBlock() { }

        public override void OnPlace() {

        }

        public override void OnLoad() {
            obj = gameObject.GetComponent<PipeObject>();
            UpdateShape();
            UpdateNeighbors();
        }

        public override void OnNeighborUpdate(Block block) {
            UpdateShape();
        }

        void UpdateShape() {
            bool[] solid = new bool[6];
            for (int i = 0; i < 6; i++) {
                Coord3 pos = position + Coord3.Directions[i];
                var block = chunk.GetBlock(pos, true);
                solid[i] = block != null && block.id == id;
            }
            ChangeShape(solid);
        }

        void ChangeShape(bool[] sol) {

            // Straight pipes
            if (sol[0] && sol[1]) SetShape(PipeType.Straight, new Coord3(0, 0, 0));
            else if (sol[2] && sol[3]) SetShape(PipeType.Straight, new Coord3(1, 0, 0));
            else if (sol[4] && sol[5]) SetShape(PipeType.Straight, new Coord3(0, 1, 0));

            // Bent
            else if (sol[BlockFace.front]) {
                if (sol[BlockFace.top]) SetShape(PipeType.Corner, new Coord3(0, 0, 0));
                else if (sol[BlockFace.right]) SetShape(PipeType.Corner, new Coord3(0, 0, -1));
                else if (sol[BlockFace.bottom]) SetShape(PipeType.Corner, new Coord3(0, 0, -2));
                else if (sol[BlockFace.left]) SetShape(PipeType.Corner, new Coord3(0, 0, -3));
                else SetShape(PipeType.End, new Coord3(0, 0, 0));
            } else if (sol[BlockFace.back]) {
                if (sol[BlockFace.top]) SetShape(PipeType.Corner, new Coord3(0, 2, 0));
                else if (sol[BlockFace.right]) SetShape(PipeType.Corner, new Coord3(0, 2, 1));
                else if (sol[BlockFace.bottom]) SetShape(PipeType.Corner, new Coord3(0, 2, 2));
                else if (sol[BlockFace.left]) SetShape(PipeType.Corner, new Coord3(0, 2, 3));
                else SetShape(PipeType.End, new Coord3(0, 2, 0));
            } 
            
            // End
            else {
                if (sol[BlockFace.top]) SetShape(PipeType.End, new Coord3(-1, 0, 0));
                else if (sol[BlockFace.right]) SetShape(PipeType.End, new Coord3(0, 1, 0));
                else if (sol[BlockFace.bottom]) SetShape(PipeType.End, new Coord3(1, 0, 0));
                else if (sol[BlockFace.left]) SetShape(PipeType.End, new Coord3(0, -1, 0));
                
                // None
                else SetShape(PipeType.End, new Coord3(1, 0, 0));
            }
        }

        void SetShape(PipeType type, Coord3 rotation) {
            this.type = type;
            this.rotation = rotation;
            obj.SetType(type, rotation);
        }

        [System.Serializable]
        public enum PipeType {
            Straight,
            Corner,
            End
        }
    }

}