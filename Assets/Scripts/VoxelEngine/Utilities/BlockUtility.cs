using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Internal;

namespace VoxelEngine.Utilities {

    public static class BlockUtility {
        /// Face and texture index rotation
        private static readonly int[] zRot = { BlockFace.top, BlockFace.left, BlockFace.bottom, BlockFace.right },
            yRot = { BlockFace.front, BlockFace.right, BlockFace.back, BlockFace.left },
            xRot = { BlockFace.front, BlockFace.top, BlockFace.back, BlockFace.bottom };

        private static readonly int[][] zFace = { new [] { 3, 1, 3, 3, 3, 3 }, new [] { 2, 2, 2, 2, 2, 2, }, new [] { 1, 3, 1, 1, 1, 1 } },
            yFace = { new [] { 0, 0, 3, 1, 0, 0 }, new [] { 0, 0, 2, 2, 0, 0 }, new [] { 0, 0, 1, 3, 0, 0, } },
            xFace = { new [] { 0, 2, 0, 2, 1, 3 }, new [] { 2, 2, 0, 2, 2, 2 }, new [] { 0, 2, 2, 0, 3, 1 } };

        public static(int index, int face) Rotate(int dir, Coord3 rot) {
            rot = rot % 4;
            int frot = 0;
            if (rot.z != 0) {
                if (dir != BlockFace.front && dir != BlockFace.back) {
                    var ind = zRot.IndexOf(dir);
                    dir = zRot[(rot.z + ind + 4) % 4];
                }
                frot += zFace[rot.z < 0 ? 3 + rot.z : rot.z - 1][dir];
            }
            if (rot.y != 0) {
                if (dir != BlockFace.top && dir != BlockFace.bottom) {
                    var ind = yRot.IndexOf(dir);
                    dir = yRot[(rot.y + ind + 4) % 4];
                }
                frot += yFace[rot.y < 0 ? 3 + rot.y : rot.y - 1][dir];
            }
            if (rot.x != 0) {
                if (dir != BlockFace.right && dir != BlockFace.left) {
                    var ind = xRot.IndexOf(dir);
                    dir = xRot[(rot.x + ind + 4) % 4];
                }
                frot += xFace[rot.x < 0 ? 3 + rot.x : rot.x - 1][dir];
            }
            return (index: dir, face: frot);
        }
    }

}