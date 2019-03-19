using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VoxelEngine {

    [CreateAssetMenu(fileName = "Block Data")]
    public class BlockData : ScriptableObject {
        public SubMesh subMesh;
        public bool collision;
        public BlockMeshType meshType;
        public float boundingSize = 1;
        public GameObject prefab;
        public string behaviour;
        public string dataType;

        public BlockPlacingMode placementType;
        public BlockType blockType;

        [DrawIf("blockType", BlockType.Ore)]
        public float spawnFrequency;
        [DrawIf("blockType", BlockType.Ore)]
        public int spawnDensity;

        [HideInInspector]
        public int[] textureIndices;
        [HideInInspector]
        public TextureIndex[] textures = new TextureIndex[6];
    }

    public enum BlockMeshType {
        Cube,
        DecalCross,
        DecalHash,
        DecalFlat,
        Air,
        Custom
    }

    public enum BlockType {
        Terrain,
        Decoration,
        Ore
    }

    public enum BlockPlacingMode {
        Zero,
        RotationalDirectional,
        RotationalInvertDirectional,
        RotationalOnly,
        DirectionalOnly,
        InvertDirectionalOnly,
    }

    public enum SubMesh {
        Opaque = 0,
        Transparent = 1
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(BlockData))]
    public class BlockDataEditor : Editor {
        BlockData bd;

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Textures");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(65);
            EditorGUILayout.LabelField("top", GUILayout.Width(25));
            bd.textures[2] = (TextureIndex) EditorGUILayout.ObjectField(bd.textures[2], typeof(TextureIndex), true);
            GUILayout.Space(90);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("left", GUILayout.Width(20));
            bd.textures[4] = (TextureIndex) EditorGUILayout.ObjectField(bd.textures[4], typeof(TextureIndex), true);
            EditorGUILayout.LabelField("front", GUILayout.Width(30));
            bd.textures[0] = (TextureIndex) EditorGUILayout.ObjectField(bd.textures[0], typeof(TextureIndex), true);
            EditorGUILayout.LabelField("right", GUILayout.Width(30));
            bd.textures[5] = (TextureIndex) EditorGUILayout.ObjectField(bd.textures[5], typeof(TextureIndex), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50);
            EditorGUILayout.LabelField("bottom", GUILayout.Width(45));
            bd.textures[3] = (TextureIndex) EditorGUILayout.ObjectField(bd.textures[3], typeof(TextureIndex), true);
            GUILayout.Space(90);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(65);
            EditorGUILayout.LabelField("back", GUILayout.Width(30));
            bd.textures[1] = (TextureIndex) EditorGUILayout.ObjectField(bd.textures[1], typeof(TextureIndex), true);
            GUILayout.Space(90);
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) {
                bd.textureIndices = new int[6];
                for (int i = 0; i < bd.textures.Length; i++) {
                    bd.textureIndices[i] = bd.textures[i]?.index ?? 0;
                }

                EditorUtility.SetDirty(bd);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void OnEnable() {
            bd = (BlockData) target;
        }
    }

#endif
}