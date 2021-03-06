﻿using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using VoxelEngine.Interfaces;
using VoxelEngine.Internal;
using VoxelEngine.Utilities;

namespace VoxelEngine {

    public struct Coord3 : System.IEquatable<Coord3>, ISerializeable {
        public int x;
        public int y;
        public int z;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coord3 BlockToWorld(Coord3 chunkWorldPos) => this + chunkWorldPos;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coord3 WorldToBlock(Coord3 chunkWorldPos) => this - chunkWorldPos;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coord3 WorldToChunk() => this.FloorDiv(ChunkSection.Size);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coord3 ModWrap(int m) => new Coord3(BlockUtil.ModWrap(x, m), BlockUtil.ModWrap(y, m), BlockUtil.ModWrap(z, m));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coord3 FloorDiv(int i) =>
            new Coord3(Mathf.FloorToInt((float) x / i), Mathf.FloorToInt((float) y / i), Mathf.FloorToInt((float) z / i));

        public bool InRange(int incLower, int excUpper) =>
            x >= incLower && x < excUpper && y >= incLower &&
            y < excUpper && z >= incLower && z < excUpper;

        public float magnitude { get => Mathf.Sqrt((float) (x * x + y * y + z * z)); }
        public int sqrMagnitude { get => x * x + y * y + z * z; }

        public static Coord3 FloorToInt(Vector3 v) =>
            new Coord3(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));

        public static float Distance(Coord3 a, Coord3 b) => (a - b).magnitude;
        public static float SqrDistance(Coord3 a, Coord3 b) => (a - b).sqrMagnitude;

        public static Coord3 RaycastToBlock(in RaycastHit hit, bool adjacent) =>
            Coord3.FloorToInt(hit.point + Vector3.one * 0.5f +
                (adjacent ? -Vector3.Max(hit.normal, Vector3.zero) : Vector3.Min(hit.normal, Vector3.zero)));

        public void Serialize(BinaryWriter writer) {
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }

        public void Deserialize(BinaryReader reader) {
            x = reader.ReadInt32();
            y = reader.ReadInt32();
            z = reader.ReadInt32();
        }

        #region operators

        public static Coord3 operator +(Coord3 a, Coord3 b) =>
            new Coord3(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Coord3 operator -(Coord3 a, Coord3 b) =>
            new Coord3(a.x - b.x, a.y - b.y, a.z - b.z);

        public static Coord3 operator -(Coord3 c) =>
            new Coord3(-c.x, -c.y, -c.z);

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

        public static Coord3 operator %(Coord3 a, int b) =>
            new Coord3(a.x % b, a.y % b, a.z % b);

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

        /// (0, 0, 0)
        public static Coord3 zero { get => s_Zero; }
        /// (1, 1, 1)
        public static Coord3 one { get => s_One; }
        /// (0, 0, 1)
        public static Coord3 forward { get => s_Forward; }
        /// (0, 0, -1)
        public static Coord3 backward { get => s_Backward; }
        /// (0, 1, 0)
        public static Coord3 up { get => s_Up; }
        /// (0, -1, 0)
        public static Coord3 down { get => s_Down; }
        /// (1, 0, 0)
        public static Coord3 right { get => s_Right; }
        /// (-1, 0, 0)
        public static Coord3 left { get => s_Left; }

        private static readonly Coord3 s_Zero = new Coord3(0, 0, 0);
        private static readonly Coord3 s_One = new Coord3(1, 1, 1);
        private static readonly Coord3 s_Forward = new Coord3(0, 0, 1);
        private static readonly Coord3 s_Backward = new Coord3(0, 0, -1);
        private static readonly Coord3 s_Up = new Coord3(0, 1, 0);
        private static readonly Coord3 s_Down = new Coord3(0, -1, 0);
        private static readonly Coord3 s_Right = new Coord3(1, 0, 0);
        private static readonly Coord3 s_Left = new Coord3(-1, 0, 0);

        public static readonly Coord3[] Directions = { forward, backward, up, down, right, left };
    }

}