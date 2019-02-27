using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextureAtlasGenerator : MonoBehaviour {
    public int res;
    public Texture2D[] textures;
    public bool alphaIsT = true;
    public bool filterModePoint = true;

    public string pathName;
    public bool overwriteFile;

    public Texture2D texture;
    public int atlasSize;

    public void Generate() {
        int size = (int)Mathf.Pow(2, Mathf.CeilToInt(Log2(Mathf.CeilToInt(Mathf.Sqrt(textures.Length)))));
        var def = DefaultTexture(res);
        atlasSize = size * res;
        texture = new Texture2D(atlasSize, atlasSize);
        Color[] tex;

        int x = 0, y = 0;
        for (int i = 0; i < size * size; i++) {
            tex = i < textures.Length ? textures[i].GetPixels() : def;
            texture.SetPixels(x * res, y * res, res, res, tex);
            (x, y) = NextCoord(x, y, size);
        }
        if (alphaIsT) texture.alphaIsTransparency = true;
        if (filterModePoint) texture.filterMode = FilterMode.Point;
        texture.Apply();
    }

    public void Save() {
        Generate();
        var path = Application.dataPath + pathName;
        var bytes = texture.EncodeToPNG();
        if (overwriteFile || !File.Exists(path)) {
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
        }
    }

    Color[] DefaultTexture(int size) {
        Color[] cols = new Color[size * size];
        bool color = true;
        int change = size / 8;

        for (int i = 0; i < size * size; i++) {
            if (i % change == 0) color = !color;
            cols[i] = color ? Color.magenta : Color.black;
        }
        return cols;
    }

    (int, int) NextCoord(int x, int y, int size) {
        if (y == 0) 
            return (0, x + 1);
        if (x < y) 
            return (x + 1, y);
        return (x, y - 1);
    }

    float Log2(float f) {
        return Mathf.Log10(f) / Mathf.Log10(2);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextureAtlasGenerator))]
public class TextureAtlasGeneratorEditor : Editor {
    TextureAtlasGenerator atlas;

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
        atlas.res = EditorGUILayout.IntField("Square Resolution", atlas.res);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textures"), true);
        atlas.alphaIsT = EditorGUILayout.Toggle("Alpha Is Transparency", atlas.alphaIsT);
        atlas.filterModePoint = EditorGUILayout.Toggle("Point Filter Mode", atlas.filterModePoint);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("File", EditorStyles.boldLabel);
        atlas.pathName = EditorGUILayout.TextField("Path Name", atlas.pathName);
        atlas.overwriteFile = EditorGUILayout.Toggle("Overwrite File", atlas.overwriteFile);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        EditorGUILayout.ObjectField("Preview Image", atlas.texture, typeof(Texture2D), false);
        EditorGUILayout.LabelField("Atlas Resolution", atlas.atlasSize + "x" + atlas.atlasSize, GUILayout.MinWidth(10));

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Preview")) atlas.Generate();
        if (GUILayout.Button("Generate")) atlas.Save();
        EditorGUILayout.EndHorizontal();
    }

    void OnEnable() {
        atlas = (TextureAtlasGenerator) target;
    }
}
#endif