using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.Pooling {

    public abstract class PrefabPool<T> : MonoBehaviour where T : MonoBehaviour {
        public GameObject prefab;
        public int startingSize;

        private List<T> available;
        private List<T> used;

        public abstract void CleanUp(T go);

        public abstract T Create();

        public T GetObject() {
            lock(available) {
                if (available.Count > 0) {
                    var go = available[0];
                    used.Add(go);
                    available.RemoveAt(0);
                    go.gameObject.SetActive(true);
                    return go;
                } else {
                    var go = Create();
                    used.Add(go);
                    return go;
                }
            }
        }

        public void ReleaseObject(T go) {
            go.gameObject.SetActive(false);
            CleanUp(go);

            lock(available) {
                available.Add(go);
                used.Remove(go);
            }
        }
    
        void Awake() {
            available = new List<T>(startingSize);
            used = new List<T>(startingSize);
            for (int i = 0; i < startingSize; i++) {
                var go = Create();
                go.gameObject.SetActive(false);
                available.Add(go);
            }
        }
    }

}