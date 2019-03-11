#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class TextureArrayWizard : ScriptableWizard {
    public TextureIndex[] textures;

    void OnWizardCreate() {
        if (textures.Length == 0) return;
        string path = EditorUtility.SaveFilePanelInProject("Save Texture Array", "Texture Array", "asset", "Save Texture Array");
        if (path.Length == 0) return;
        var t = textures[0].texture;
        var texArray = new Texture2DArray(t.width, t.height, textures.Length, t.format, t.mipmapCount > 1);
        texArray.anisoLevel = t.anisoLevel;
        texArray.filterMode = t.filterMode;
        texArray.wrapMode = t.wrapMode;

        for (int i = 0; i < textures.Length; i++) {
            for (int m = 0; m < t.mipmapCount; m++) {
                var ti = textures[i];
                Graphics.CopyTexture(ti.texture, 0, m, texArray, ti.index, m);
            }
        }

        AssetDatabase.CreateAsset(texArray, path);
    }

    [MenuItem("Assets/Create/Texture Array")]
    static void CreateWizard() {
        ScriptableWizard.DisplayWizard<TextureArrayWizard>("Create Texture Array", "Create");
    }
}

#endif