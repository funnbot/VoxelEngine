using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "StructureData")]
public class StructureData : ScriptableObject {
    public int height;
    public Vector2Int size;
    public SerialVector3Int origin;
    public bool cutout;
    public string[] ids;
    public int opts { get => ids != null ? ids.Length : 0; }

    public Level[] levels;
    public int editLevel;

    public int fill;

    public int[] this [int level] {
        get => data[level];
    }

    public BlockData[] blocks {
        get {
            if (blockMapping == null || blockMapping.Length == 0) {
                blockMapping = new BlockData[opts];
                for (int i = 0; i < opts; i++)
                    blockMapping[i] = ResourceStore.Blocks[ids[i]];
            }
            return blockMapping;
        }
    }

    private int[][] data;
    private BlockData[] blockMapping;

    [System.Serializable]
    public struct Level {
        public Row[] row;
    }

    [System.Serializable]
    public struct Row {
        public int[] col;
    }

    void OnEnable() {
        data = new int[height][];
        for (int z = 0; z < height; z++) {
            data[z] = new int[size.y * size.x];
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    int val = levels[z].row[y].col[x];
                    data[z][y * size.y + x] = val;
                }
            }
        }
    }

    public void Reset() {
        if (height == 0) height = 1;
        if (size == Vector2Int.zero) size = Vector2Int.one;

        levels = new Level[height];
        for (int z = 0; z < height; z++) {
            levels[z] = new Level();
            levels[z].row = new Row[size.y];
            for (int y = 0; y < size.y; y++) {
                levels[z].row[y] = new Row();
                levels[z].row[y].col = new int[size.x];
                for (int x = 0; x < size.x; x++) {
                    levels[z].row[y].col[x] = 0;
                }
            }
        }
    }

    public int GetValue(int y, int x) =>
        levels[editLevel].row[y].col[x];
    public void SetValue(int y, int x, int val) =>
        levels[editLevel].row[y].col[x] = val;

    [System.Serializable]
    public class SerialVector3Int {
        public int x, y, z;
        public SerialVector3Int(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static explicit operator Vector3Int(SerialVector3Int v) {
            return new Vector3Int(v.x, v.y, v.z);
        }

        public static explicit operator SerialVector3Int(Vector3Int v) {
            return new SerialVector3Int(v.x, v.y, v.z);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(StructureData))]
public class StructureDataEditor : Editor {
    StructureData data;

    public override void OnInspectorGUI() {
        if (data.levels == null) data.Reset();

        serializedObject.Update();

        // Dimensions
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Dimensions", EditorStyles.boldLabel);
        data.height = EditorGUILayout.IntField("Height", Mathf.Max(data.height, 1));
        data.size = EditorGUILayout.Vector2IntField("Size", Vector2Int.Max(data.size, Vector2Int.one));
        if (EditorGUI.EndChangeCheck()) data.Reset();
        data.origin = (StructureData.SerialVector3Int)EditorGUILayout.Vector3IntField(new GUIContent("Origin", "Starting from bottom left (1, 1, 1)"), Vector3Int.Max((Vector3Int)data.origin, Vector3Int.one));
        EditorGUILayout.Space();
        data.cutout = EditorGUILayout.Toggle("Cutout Region", data.cutout);

        // Block ID Mapping
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ids"), new GUIContent("Block ID Mapping", "Element number corresponds to the value used in the table"), true);
        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        EditorGUILayout.Space();

        // Editor
        EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pick Level", GUILayout.Width(70));
        if (GUILayout.Button("«", GUILayout.Width(20))) data.editLevel--;
        if (GUILayout.Button("»", GUILayout.Width(20))) data.editLevel++;
        data.editLevel = Mathf.Clamp(EditorGUILayout.IntField(data.editLevel + 1, GUILayout.Width(30)), 1, data.height) - 1;

        EditorGUILayout.Space();
        if (GUILayout.Button("Fill", GUILayout.Width(30))) {
            for (int y = 0; y < data.size.y; y++) {
                for (int x = 0; x < data.size.x; x++) {
                    data.SetValue(y, x, data.fill);
                }
            }
        }
        data.fill = Mathf.Clamp(EditorGUILayout.IntField(data.fill, GUILayout.Width(30)), 0, data.opts - 1);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        for (int y = 0; y < data.size.y; y++) {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < data.size.x; x++) {
                int val = data.GetValue(y, x);
                val = EditorGUILayout.IntField(val, GUILayout.Width(14), GUILayout.Height(14));
                data.SetValue(y, x, Mathf.Clamp(val, 0, data.opts - 1));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();

        if (GUILayout.Button("Reset", GUILayout.Width(60))) data.Reset();
    }

    void OnEnable() {
        data = (StructureData) target;
    }
}
#endif