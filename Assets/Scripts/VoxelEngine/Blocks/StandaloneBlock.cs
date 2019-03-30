using UnityEngine;
using Ceras;

namespace VoxelEngine.Blocks {

    public class StandaloneBlock : Block {
        [Exclude]
        public GameObject gameObject;

        public StandaloneBlock(Block block) : base(block) { }
        public StandaloneBlock() { }
    }

}