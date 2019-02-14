using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FastDebug {
    public static int count;
    static List<string> store = new List<string>();

    public static void Log(params string[] m) {
        store.AddRange(m);
        if (store.Count >= count) {
            string o = "";
            foreach (var s in store) {
                o += s + "\n\n";
            }
            Debug.Log(o);
            store.Clear();
        }
    }
}