using Ceras;
using UnityEngine;

namespace VoxelEngine.Blocks {

    public class StandaloneBlock : Block {
        [Exclude]
        public GameObject gameObject;

        public StandaloneBlock(Block copy) : base(copy) {
            
        }
        public StandaloneBlock() { }
    }

}