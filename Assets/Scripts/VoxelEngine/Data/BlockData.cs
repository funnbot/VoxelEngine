﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VoxelEngine.Data {

    [CreateAssetMenu(fileName = "Block Data")]
    public class BlockData : ScriptableObject {
        [System.NonSerialized]
        public string blockId;
        [System.NonSerialized]
        public byte byteId;

        [Space]
        [Header("Info")]
        public string blockName;
        public string description;
        public Texture2D icon;
        [Space]
        public string dataType;
        [Space]
        [Header("Mesh Generation")]
        public BlockType blockType;
        public GameObject prefab;
        [Space]
        public bool collision;
        public float boundingSize = 1;
        public SubMesh subMesh;
        [Space]
        public bool rotation;
        [Space]
        [Header("Interaction")]
        public BlockPlacingMode placementType;
        [Space]
        public bool stackable;
        public bool placeable;
        [Space]
        [Header("Procedural Generation")]
        public BlockSpawnType spawnType;
        [Space]
        [DrawIf("spawnType", BlockSpawnType.Ore)]
        public float spawnFrequency;
        [DrawIf("spawnType", BlockSpawnType.Ore)]
        public int spawnDensity;

        [HideInInspector]
        public int[] textureIndices;
        [HideInInspector]
        public TextureIndex[] textures = new TextureIndex[6];
    }

    [System.Serializable]
    public enum BlockType {
        Entity,

        Cube,
        DecalCross,
        DecalHash,
        DecalFlat,
        Custom
    }

    [System.Serializable]
    public enum BlockSpawnType {
        Terrain,
        Decoration,
        Ore
    }

    [System.Serializable]
    public enum BlockPlacingMode {
        Zero,
        RotationalDirectional,
        RotationalInvertDirectional,
        RotationalOnly,
        DirectionalOnly,
        InvertDirectionalOnly,
    }

    [System.Serializable]
    public enum SubMesh {
        Opaque = 0,
        Transparent = 1
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(BlockData))]
    public class BlockDataEditor : Editor {
        BlockData bd;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

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