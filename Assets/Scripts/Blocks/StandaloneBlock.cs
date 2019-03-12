using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class StandaloneBlock : Block {
        public GameObject gameObject;

        public StandaloneBlock(Block block) : base(block) { }
    }

}