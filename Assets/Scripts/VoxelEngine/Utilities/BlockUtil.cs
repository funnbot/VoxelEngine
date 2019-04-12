using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Internal;

namespace VoxelEngine.Utilities {

    public static class BlockUtil {
        public static int ModWrap(int x, int max) {
            int r = x % max;
            return r < 0 ? r + max : r;
        }
    }

}