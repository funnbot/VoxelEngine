using UnityEngine;

namespace VoxelEngine {

    public struct Coord2 : System.IEquatable<Coord2> {
        public int x;
        public int y;

        public Coord2(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static Coord2 FloorToInt(Vector2 v) =>
        new Coord2(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));

        public float magnitude { get => Mathf.Sqrt((float) (x * x + y * y)); }
        public int sqrMagnitude { get => x * x + y * y; }

        public static float Distance(Coord2 a, Coord2 b) => (a - b).magnitude;
        public static float SqrDistance(Coord2 a, Coord2 b) => (a - b).sqrMagnitude;

        #region operators

        public static Coord2 operator +(Coord2 a, Coord2 b) =>
        new Coord2(a.x + b.x, a.y + b.y);

        public static Coord2 operator -(Coord2 a, Coord2 b) =>
        new Coord2(a.x - b.x, a.y - b.y);

        public static Coord2 operator *(Coord2 a, Coord2 b) =>
        new Coord2(a.x * b.x, a.y * b.y);

        public static Coord2 operator *(Coord2 a, int b) =>
        new Coord2(a.x * b, a.y * b);

        public static Coord2 operator /(Coord2 a, Coord2 b) =>
        new Coord2(a.x / b.x, a.y / b.y);

        public static Coord2 operator /(Coord2 a, int b) =>
        new Coord2(a.x / b, a.y / b);

        public static bool operator ==(Coord2 lhs, Coord2 rhs) =>
        lhs.x == rhs.x && lhs.y == rhs.y;

        public static bool operator !=(Coord2 lhs, Coord2 rhs) =>
        !(lhs == rhs);

        public static implicit operator Coord2(Vector2 v) =>
        new Coord2((int) v.x, (int) v.y);

        public static implicit operator Coord2(Vector2Int v) =>
        new Coord2(v.x, v.y);

        public static explicit operator Coord2(Coord3 c) =>
        new Coord2(c.x, c.z);

        #endregion operators

        #region equality

        public override string ToString() {
            return System.String.Format("({0},{1})", x, y);
        }

        public override bool Equals(object other) {
            if (!(other is Coord2)) return false;
            return Equals((Coord2) other);
        }

        public bool Equals(Coord2 other) =>
        x.Equals(other.x) && y.Equals(other.y);

        public override int GetHashCode() =>
        x.GetHashCode() ^ (y.GetHashCode() << 2);

        #endregion equality

        /// (0, 0)
        public static Coord2 zero { get => s_Zero; }
        /// (1, 1)
        public static Coord2 one { get => s_One; }
        /// (0, 1)
        public static Coord2 up { get => s_Up; }
        /// (0, -1)
        public static Coord2 down { get => s_Down; }
        /// (1, 0)
        public static Coord2 right { get => s_Right; }
        /// (-1, 0)
        public static Coord2 left { get => s_Left; }

        /// (-1, 1)
        public static Coord2 topLeft { get => s_TopLeft; }
        /// (1, 1)
        public static Coord2 topRight { get => s_TopRight; }
        /// (1, -1)
        public static Coord2 bottomRight { get => s_BottomRight; }
        /// (-1, -1)
        public static Coord2 bottomLeft { get => s_BottomLeft; }

        private static readonly Coord2 s_Zero = new Coord2(0, 0);
        private static readonly Coord2 s_One = new Coord2(1, 1);
        private static readonly Coord2 s_Up = new Coord2(0, 1);
        private static readonly Coord2 s_Down = new Coord2(0, -1);
        private static readonly Coord2 s_Right = new Coord2(1, 0);
        private static readonly Coord2 s_Left = new Coord2(-1, 0);

        private static readonly Coord2 s_TopLeft = new Coord2(-1, 1);
        private static readonly Coord2 s_TopRight = new Coord2(1, 1);
        private static readonly Coord2 s_BottomRight = new Coord2(1, -1);
        private static readonly Coord2 s_BottomLeft = new Coord2(-1, -1);

        public static readonly Coord2[] Directions = { up, down, right, left };
        public static readonly Coord2[] TileCorners = { up, one, right, zero };

    }

}