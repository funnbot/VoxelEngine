using System.Collections.Generic;

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
    }

}