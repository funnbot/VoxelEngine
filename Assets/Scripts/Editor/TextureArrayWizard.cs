using System.IO;
using UnityEditor;
using UnityEngine;
using VoxelEngine.Data;

public class TextureArrayWizard : ScriptableWizard {
    public string folder;

    void OnWizardCreate() {
        var textures = Resources.LoadAll<TextureIndex>("Textures/" + folder);
        Debug.Log(textures.Length);

        if (textures.Length == 0) return;
        string path = EditorUtility.SaveFilePanelInProject("Save Texture Array", "Texture Array", "asset", "Save Texture Array", "Resources/Textures/TextureArrays");
        if (path.Length == 0) return;
        var t = textures[0].texture;
        var texArray = new Texture2DArray(t.width, t.height, textures.Length, t.format, t.mipmapCount > 1);
        texArray.anisoLevel = t.anisoLevel;
        texArray.filterMode = t.filterMode;
        texArray.wrapMode = t.wrapMode;

        for (int i = 0; i < textures.Length; i++) {
            var ti = textures[i];
            ti.index = i;
            EditorUtility.SetDirty(ti);
            for (int m = 0; m < t.mipmapCount; m++) {
                Graphics.CopyTexture(ti.texture, 0, m, texArray, i, m);
            }
        }

        AssetDatabase.CreateAsset(texArray, path);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Create/Texture Array")]
    static void CreateWizard() {
        ScriptableWizard.DisplayWizard<TextureArrayWizard>("Create Texture Array", "Create");
    }
}