using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class MovingBehaviour : BlockBehaviour {
        public static readonly string name = "moving";

        protected override void UpdateBlock(Block block) {
            var sd = (StandaloneBlock)block;
            sd.gameObject.transform.Translate(new Vector3(0, 0.01f, 0f));
        }
    }

}