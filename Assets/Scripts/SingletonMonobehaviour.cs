using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    protected virtual void AwakeImpl() { }

    public static T Instance;
    private void Awake() {
        if (Instance != null) {
            Debug.LogError($"Destroying duplicate of singleton instance: {typeof(T)}");
            Destroy(gameObject);
        } else {
            Instance = (T)FindObjectOfType(typeof(T));
            AwakeImpl();
        }
    }
}