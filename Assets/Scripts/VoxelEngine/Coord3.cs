using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VoxelEngine {

    [System.Serializable]
    public struct Coord3 : System.IEquatable<Coord3> {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public Coord3(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Coord3(Coord2 c, int z) {
            x = c.x;
            y = c.y;
            this.z = z;
        }

        public Coord3(int x, int y) {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        // Rotation

        public int IndexRotation(int dir) {
            dir = RotateInt(dir, x, 0);
            dir = RotateInt(dir, y, 1);
            dir = RotateInt(dir, z, 2);
            return dir;

            // x = 0
            // up, down, right, left, forw, back
            // 0, 1, 2, 3, 4, 5
            // x = 1
            // back, forw, right, left, up, down
            // 0-5, 1-4, 2, 3, 4-0, 5-1
            // x = 2
            // down, up, right, left, back, forw
            // 0-1, 1-0, 2, 3, 4-5, 5-4
            // x = 3
            // forw, back, right, left, down, up
            // 0-4, 1-5, 2, 3, 4-1, 5-0

            // y = 0
            // up, down, right, left, forw, back
            // 0, 1, 2, 3, 4, 5
            // y = 1
            // up, down, forw, back, left, right
            // y = 2
            // up, down, left, right, back, forw
            // y = 3
            // up, down, back, forw, right, left

            // z = 0
            // up, down, right, left, forw, back
            // 0, 1, 2, 3, 4, 5
            // z = 1
            // left, right, up, down, forw, back
            // z = 2
            // down, up, left, right, forw, back
            // z = 3
            // right, left, down, up, forw, back
        }

        private int RotateInt(int dir, int rot, int i) {
            int[] xRot = { FaceDir.up, FaceDir.forward, FaceDir.down, FaceDir.backward };
            int[] yRot = { FaceDir.right, FaceDir.forward, FaceDir.left, FaceDir.backward };
            int[] zRot = { FaceDir.up, FaceDir.right, FaceDir.down, FaceDir.left };

            if (i == 0 && x != 0) {
                if (dir < 2 || dir > 3) {
                    var ind = xRot.TakeWhile(n => n != dir).Count();
                    return xRot[(ind + rot + 4) % 4];
                }
            } else if (i == 1 && y != 0) {
                if (dir > 1) {
                    var ind = yRot.TakeWhile(n => n != dir).Count();
                    return yRot[(ind + rot + 4) % 4];
                }
            } else if (i == 2 && z != 0) {
                if (dir < 4) {
                    var ind = zRot.TakeWhile(n => n != dir).Count();
                    return zRot[(ind + rot + 4) % 4];
                }
            }
            return dir;
        }

        public int FaceRotation(int dir) {
            switch (dir) {
                default:
                case 2: return x;
                case 3: return -x;
                case 0: return y;
                case 1: return -y;
                case 4: return z;
                case 5: return -z;
            }
            /* 
            if (dir == 2 || dir == 3)
                return x;
            if (dir == 0 || dir == 1)
                return y;
            // if (dir == 4 || dir == 5)
            return z;*/
        }

        // Rotation

        public Coord3 FloorDiv(int i) =>
        new Coord3(Mathf.FloorToInt((float) x / i), Mathf.FloorToInt((float) y / i), Mathf.FloorToInt((float) z / i));

        public bool InRange(int incLower, int excUpper) =>
        x >= incLower && x < excUpper && y >= incLower &&
        y < excUpper && z >= incLower && z < excUpper;

        public Coord3 ToChunkPos() =>
        this / Chunk.Size;

        public Coord3 ToWorldPos(Coord3 chunkWorldPos) =>
        this + chunkWorldPos;

        public Coord3 ToBlockPos(Coord3 chunkWorldPos) =>
        this - chunkWorldPos;

        public int MaxElem() =>
        Mathf.Max(x, Mathf.Max(y, z));

        public int MinElem() =>
        Mathf.Min(x, Mathf.Min(y, z));

        public float magnitude { get => Mathf.Sqrt((float) (x * x + y * y + z * z)); }

        public int sqrMagnitude { get => x * x + y * y + z * z; }

        public static Coord3 FloorToInt(Vector3 v) =>
        new Coord3(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));

        public static float Distance(Coord3 a, Coord3 b) =>
        (a - b).magnitude;

        #region operators

        public static Coord3 operator +(Coord3 a, Coord3 b) =>
        new Coord3(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Coord3 operator -(Coord3 a, Coord3 b) =>
        new Coord3(a.x - b.x, a.y - b.y, a.z - b.z);

        public static Coord3 operator *(Coord3 a, Coord3 b) =>
        new Coord3(a.x * b.x, a.y * b.y, a.z * b.z);

        public static Coord3 operator *(Coord3 a, int b) =>
        new Coord3(a.x * b, a.y * b, a.z * b);

        public static Coord3 operator /(Coord3 a, Coord3 b) =>
        new Coord3(a.x / b.x, a.y / b.y, a.z / b.z);

        public static Coord3 operator /(Coord3 a, int b) =>
        new Coord3(a.x / b, a.y / b, a.z / b);

        public static Coord3 operator /(Coord3 a, float b) =>
        new Coord3(Mathf.FloorToInt(a.x / b), Mathf.FloorToInt(a.y / b), Mathf.FloorToInt(a.z / b));

        public static bool operator ==(Coord3 lhs, Coord3 rhs) =>
        lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;

        public static bool operator !=(Coord3 lhs, Coord3 rhs) =>
        !(lhs == rhs);

        public static implicit operator Coord3(Vector3 v) =>
        new Coord3((int) v.x, (int) v.y, (int) v.z);

        public static implicit operator Vector3(Coord3 v) =>
        new Vector3(v.x, v.y, v.z);

        public static implicit operator Coord3(Vector3Int v) =>
        new Coord3(v.x, v.y, v.z);

        public static explicit operator Coord3(Coord2 c) =>
        new Coord3(c.x, 0, c.y);

        #endregion operators

        #region equality

        public override string ToString() {
            return System.String.Format("({0}, {1}, {2})", x, y, z);
        }

        public override bool Equals(object other) {
            if (!(other is Coord3)) return false;
            return Equals((Coord3) other);
        }

        public bool Equals(Coord3 other) =>
        this == other;

        public override int GetHashCode() {
            var yHash = y.GetHashCode();
            var zHash = z.GetHashCode();
            return x.GetHashCode() ^ (yHash << 4) ^ (yHash >> 28) ^ (zHash >> 4) ^ (zHash << 28);
        }

        #endregion equality

        public static Coord3 zero { get => s_Zero; }
        public static Coord3 one { get => s_One; }
        public static Coord3 up { get => s_Up; }
        public static Coord3 down { get => s_Down; }
        public static Coord3 right { get => s_Right; }
        public static Coord3 left { get => s_Left; }
        public static Coord3 forward { get => s_Forward; }
        public static Coord3 backward { get => s_Backward; }

        private static readonly Coord3 s_Zero = new Coord3(0, 0, 0);
        private static readonly Coord3 s_One = new Coord3(1, 1, 1);
        private static readonly Coord3 s_Up = new Coord3(0, 1, 0);
        private static readonly Coord3 s_Down = new Coord3(0, -1, 0);
        private static readonly Coord3 s_Right = new Coord3(1, 0, 0);
        private static readonly Coord3 s_Left = new Coord3(-1, 0, 0);
        private static readonly Coord3 s_Forward = new Coord3(0, 0, 1);
        private static readonly Coord3 s_Backward = new Coord3(0, 0, -1);

        public static readonly Coord3[] Directions = { up, down, right, left, forward, backward };
        public static class FaceDir {
            public static readonly int up = 0;
            public static readonly int down = 1;
            public static readonly int right = 2;
            public static readonly int left = 3;
            public static readonly int forward = 4;
            public static readonly int backward = 5;
        }
    }

}