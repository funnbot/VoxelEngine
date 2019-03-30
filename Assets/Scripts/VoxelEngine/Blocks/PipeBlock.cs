using UnityEngine;
using VoxelEngine.Interfaces;

namespace VoxelEngine.Blocks {

    public class PipeBlock : StandaloneBlock {
        PipeObject obj;

        public PipeType type;
        public Coord3 rotation;

        public PipeBlock(Block block) : base(block) { }
        public PipeBlock() { }

        public override void OnPlace() {
            UpdateShape();
        }

        public override void OnLoad() {
            obj = gameObject.GetComponent<PipeObject>();
        }

        public override void OnNeighborUpdate(Block block) {
            UpdateShape();
        }

        void UpdateShape() {
            bool[] solid = new bool[6];
            for (int i = 0; i < 6; i++) {
                Coord3 pos = position + Coord3.Directions[i];
                var block = chunk.GetBlock(pos);
                solid[i] = block == null;
            }
            ChangeShape(solid);
        }

        void ChangeShape(bool[] sol) {
            if (sol[0] && sol[1]) obj.SetType(PipeType.Straight, new Coord3(0, 1, 0));
        }

        [System.Serializable]
        public enum PipeType {
            Straight,
            Corner,
            End
        }
    }

}