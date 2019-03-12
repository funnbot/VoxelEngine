using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class MovingBehaviour : BlockBehaviour<StandaloneBlock> {
        public static readonly string name = "moving";

        protected override void UpdateBlock(StandaloneBlock block) {
            block.gameObject.transform.Translate(new Vector3(0, 0.01f, 0));
        }
    }

}