using System.Collections.Generic;
using System.IO;

namespace VoxelEngine {

    public static class Extensions {
        public static int IndexOf(this int[] arr, int value) {
            int len = arr.Length;
            for (int i = 0; i < len; i++) {
                if (arr[i] == value) return i;
            }
            return -1;
        }
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>
            dictionary.TryGetValue(key, out var ret) ? ret : default;
            

        public static void Write(this BinaryWriter writer, Coord3 coord) {
            writer.Write(coord.x);
            writer.Write(coord.y);
            writer.Write(coord.z);
        }

        public static Coord3 ReadCoord3(this BinaryReader reader) {
            Coord3 result;
            result.x = reader.ReadInt32();
            result.y = reader.ReadInt32();
            result.z = reader.ReadInt32();
            return result;
        }
    }

}