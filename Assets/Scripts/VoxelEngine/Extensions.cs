using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelEngine {

    public static class Extensions {
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
    }

}