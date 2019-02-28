using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PrefabPool : MonoBehaviour {
    public GameObject prefab;
    public int startingSize;

    private List<GameObject> available;
    private List<GameObject> used;

    public abstract void CleanUp(GameObject go);

    public virtual GameObject Create() =>
        Instantiate(prefab);

    public GameObject GetObject() {
        lock(available) {
            if (available.Count != 0) {
                var go = available[0];
                used.Add(go);
                available.RemoveAt(0);
                go.SetActive(true);
                return go;
            } else {
                var go = Create();
                used.Add(go);
                go.SetActive(true);
                return go;
            }
        }
    }

    public void ReleaseObject(GameObject go) {
        go.SetActive(false);
        CleanUp(go);

        lock(available) {
            available.Add(go);
            used.Remove(go);
        }
    }

    void Awake() {
        available = new List<GameObject>(startingSize);
        used = new List<GameObject>(startingSize);
        for (int i = 0; i < startingSize; i++) {
            available[i] = Create();
            available[i].SetActive(false);
        }
    }
}