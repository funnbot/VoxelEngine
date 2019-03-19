using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextureArrayGenerator : MonoBehaviour {
    public Material material;
    public Texture2D[] textures;
    public bool alpha;

    public TextureIndex texture;

    public void Generate() {
        if (textures.Length <= 0) return;

        Texture2DArray texArray = new Texture2DArray(textures[0].width, textures[0].height,
            textures.Length, alpha ? TextureFormat.RGBA32 : TextureFormat.RGB24, true);

        texArray.filterMode = FilterMode.Point;
        texArray.wrapMode = TextureWrapMode.Repeat;

        for (int i = 0; i < textures.Length; i++) {
            for (int mip = 0; mip < textures[0].mipmapCount; mip++) {
                texArray.SetPixels(textures[i].GetPixels(mip), i, mip);
            }
        }

        texArray.Apply();
        // material.SetTexture("_MainTex", texArray);
    }

    void Awake() {
        if (gameObject.activeSelf) Generate();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextureArrayGenerator))]
public class TextureAtlasArrayEditor : Editor {
    TextureArrayGenerator array;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate")) array.Generate();
    }

    void OnEnable() {
        array = (TextureArrayGenerator) target;
    }
}
#endif