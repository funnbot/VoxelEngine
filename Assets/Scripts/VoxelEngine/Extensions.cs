using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

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
            
        public static IEnumerator WaitTillComplete(this Task task) {
            while (task.Status != TaskStatus.RanToCompletion) {
                if (task.IsFaulted) {
                    Debug.Log(task.Exception);
                    yield break;
                }
                yield return null;
            }
        } 

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