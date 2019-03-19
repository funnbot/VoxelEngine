using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    [System.Serializable]
    public class StandaloneBlock : Block {
        [System.NonSerialized]
        public GameObject gameObject;

        public StandaloneBlock(Block block) : base(block) { }
    }

}