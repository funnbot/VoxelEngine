using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
using VoxelEngine.Data;
using VoxelEngine.Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CodeTester : MonoBehaviour {
    public MeshFilter filter;

    public BlockData data;

    void TestCode() {
        var mesh = new MeshBuilder();
        for (int i = 0; i < 6; i++) {
            mesh.AddCubeFace(i, new Coord3(0, 0, 0), 0, data.textureIndices[i], 0);
        }

        filter.sharedMesh = mesh.ToMesh();

        // AssetDatabase.CreateAsset(filter.sharedMesh, "Assets/indexed_cube.asset");
    }

    bool runOnAwake = false;
    public void Run() {
        TestCode();
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
        c = (CodeTester) target;
    }
}
#endif