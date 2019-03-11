﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Linq;

public class CodeTester : MonoBehaviour {
    object TestCode() {
        string o = "";
        Coord3 rot = new Coord3(1, 1, 0);

        for (int i = 0; i < 6; i++) {
            o += Chunk.Rotate(i, rot) + " ";
        }
        return o;
    }




    bool runOnAwake = false;
    public void Run() {
        Debug.Log(TestCode());
    }
    void Awake() {
        if (runOnAwake) Run();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CodeTester))]
public class CodeTesterEditor : Editor {
    CodeTester c;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Run")) c.Run();
    }

    void OnEnable() {
        c = (CodeTester)target;
    }
}
#endif