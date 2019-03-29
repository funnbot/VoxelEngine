using UnityEngine;

namespace VoxelEngine.Blocks {

    [System.Serializable]
    public class StandaloneBlock : Block {
        [System.NonSerialized]
        public GameObject gameObject;

        public StandaloneBlock(Block block) : base(block) { }
        public StandaloneBlock() { }
    }

}