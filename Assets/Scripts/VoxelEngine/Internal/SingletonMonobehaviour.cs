﻿using UnityEngine;

namespace VoxelEngine.Internal {

    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
        protected virtual void AwakeImpl() { }

        public static T Instance;
        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"Destroying duplicate of singleton instance: {typeof(T)}");
                Destroy(gameObject);
            } else {
                Instance = (T) FindObjectOfType(typeof(T));
                AwakeImpl();
            }
        }
    }

}