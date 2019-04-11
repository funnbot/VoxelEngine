using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Internal;

namespace VoxelEngine.Utilities {

    public static class BlockUtility {
        /// Face and texture index rotation

        private static readonly int[] zRot = { BlockFace.top, BlockFace.right, BlockFace.bottom, BlockFace.left },
            yRot = { BlockFace.front, BlockFace.left, BlockFace.back, BlockFace.right },
            xRot = { BlockFace.front, BlockFace.top, BlockFace.back, BlockFace.bottom };

        private static readonly int[] zIndex = {-1, -1, 0, 2, 1, 3 },
            yIndex = { 0, 2, -1, -1, 3, 1 },
            xIndex = { 0, 2, 1, 3, -1, -1 };

        private static readonly int[][] zFace = { new [] { 1, 3, 1, 1, 1, 1 }, new [] { 2, 2, 2, 2, 2, 2, }, new [] { 3, 1, 3, 3, 3, 3 } },
            yFace = { new [] { 0, 0, 1, 1, 0, 0 }, new [] { 0, 0, 2, 2, 0, 0 }, new [] { 0, 0, 3, 3, 0, 0, } },
            xFace = { new [] { 0, 2, 0, 2, 1, 3 }, new [] { 2, 2, 0, 0, 2, 2 }, new [] { 0, 2, 2, 0, 3, 1 } };  

        public static int IndexRotateAround(int dir, ref Coord3 rot) {
            if (rot.z != 0) {
                var ind = zIndex[dir];
                if (ind != -1) dir = zRot[(rot.z + ind + 4) % 4];
            }
            if (rot.y != 0) {
                var ind = yIndex[dir];
                if (ind != -1) dir = yRot[(rot.y + ind + 4) % 4];
            }
            if (rot.x != 0) {
                var ind = xIndex[dir];
                if (ind != -1) dir = xRot[(rot.x + ind + 4) % 4];
            }
            return dir;
        }

        public static int FaceRotate(int dir, ref Coord3 rot) {
            int face = 0;
            if (rot.z != 0) face += zFace[rot.z - 1][dir];
            if (rot.y != 0) face += yFace[rot.y - 1][dir];
            if (rot.x != 0) face += xFace[rot.x - 1][dir];
            return face;
        }

        public static int ModWrap(int x, int max) {
            int r = x % max;
            return r < 0 ? r + max : r;
        }

        /// The UV rotation for each possible Euler rotation indexed as (roX, rotY, rotZ, dir)
        // Rotation is clockwise, 90 degree turns
        private static readonly int[,,,] FaceUVRotations = {
            {
                { { 0, 0, 0, 0, 0, 0 }, { 1, 3, 1, 1, 1, 1 }, { 2, 2, 2, 2, 2, 2 }, { 3, 1, 3, 3, 3, 3 } },
                { { 0, 0, 1, 3, 0, 0 }, { 1, 1, 2, 0, 3, 1 }, { 2, 2, 1, 1, 2, 2 }, { 3, 3, 0, 2, 1, 3 } },
                { { 0, 0, 2, 2, 0, 0 }, { 3, 1, 3, 3, 1, 1 }, { 2, 2, 0, 0, 2, 2 }, { 1, 3, 1, 1, 3, 3 } },
                { { 0, 0, 3, 1, 0, 0 }, { 1, 1, 0, 2, 1, 3 }, { 2, 2, 1, 3, 2, 2 }, { 3, 3, 2, 0, 3, 1 } } 
            },
            {
                { { 0, 2, 2, 0, 3, 1 }, { 1, 3, 1, 1, 0, 2 }, { 2, 0, 0, 2, 1, 3 }, { 3, 1, 3, 3, 2, 0 } },
                { { 3, 1, 3, 3, 2, 0 }, { 0, 2, 2, 0, 3, 1 }, { 1, 3, 1, 1, 0, 2 }, { 2, 0, 0, 2, 1, 3 } },
                { { 2, 0, 0, 2, 1, 3 }, { 3, 1, 3, 3, 2, 0 }, { 0, 2, 2, 0, 3, 1 }, { 1, 3, 1, 1, 0, 2 } },
                { { 1, 3, 1, 1, 0, 2 }, { 2, 0, 0, 2, 1, 3 }, { 3, 1, 3, 3, 2, 0 }, { 0, 2, 2, 0, 3, 1 } } 
            },
            {
                { { 2, 2, 0, 0, 2, 2 }, { 1, 3, 1, 1, 3, 3 }, { 0, 0, 2, 2, 0, 0 }, { 3, 1, 3, 3, 1, 1 } },
                { { 2, 2, 1, 3, 2, 2 }, { 3, 3, 2, 0, 3, 1 }, { 0, 0, 3, 1, 0, 0 }, { 1, 1, 0, 2, 1, 3 } },
                { { 2, 2, 2, 2, 2, 2 }, { 3, 1, 3, 3, 3, 3 }, { 0, 0, 0, 0, 0, 0 }, { 1, 3, 1, 1, 1, 1 } },
                { { 2, 2, 3, 1, 2, 2 }, { 3, 3, 0, 2, 1, 3 }, { 0, 0, 1, 3, 1, 1 }, { 1, 1, 2, 0, 3, 1 } } 
            },
            {
                { { 0, 2, 0, 2, 1, 3 }, { 1, 3, 1, 1, 2, 0 }, { 2, 0, 2, 0, 3, 1 }, { 3, 1, 3, 3, 0, 2 } },
                { { 1, 3, 1, 1, 2, 0 }, { 2, 0, 2, 0, 3, 1 }, { 3, 1, 3, 3, 0, 2 }, { 0, 2, 0, 2, 1, 3 } },
                { { 2, 1, 2, 0, 3, 1 }, { 3, 1, 3, 3, 0, 2 }, { 0, 2, 0, 2, 1, 3 }, { 1, 3, 1, 1, 2, 0 } },
                { { 3, 1, 3, 3, 0, 2 }, { 0, 2, 0, 2, 1, 3 }, { 1, 3, 1, 1, 2, 0 }, { 2, 0, 2, 0, 3, 1 } } 
            },
        };
    }

}