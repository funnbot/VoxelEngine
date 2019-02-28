using System.Globalization;
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

        public static Coord3 FloorToInt(Vector3 v) =>
        new Coord3(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));

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
        x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);

        public override int GetHashCode() {
            var yHash = y.GetHashCode();
            var zHash = z.GetHashCode();
            return x.GetHashCode() ^ (yHash << 4) ^ (yHash >> 28) ^ (zHash >> 4) ^ (zHash << 28);
        }

        #endregion equality

        public static readonly Coord3[] Directions = { up, down, right, left, forward, backward };

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
    }

}