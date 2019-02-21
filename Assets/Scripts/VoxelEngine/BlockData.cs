using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    [CreateAssetMenu(fileName = "Block Data")]
    public class BlockData : ScriptableObject {
        public bool transparent;
        public bool collision;
        public BlockMeshType meshType;
        public Mesh customMesh;
        public string behaviourType;

        // public Vector2Int[] FaceTiling = new Vector2Int[6];
        public int[] texIndices = new int[6];


        public BlockPlacingType placementType;
        public BlockType blockType;

        [DrawIf("blockType", BlockType.Ore)]
        public float spawnFrequency;
        [DrawIf("blockType", BlockType.Ore)]
        public int spawnDensity;
    }

    public enum BlockMeshType {
        Cube,
        Decal,
        Air,
        Custom
    }

    public enum BlockType {
        Terrain,
        Decoration,
        Ore
    }

    public enum BlockPlacingType {
        Zero,
        RotationalDirectional,
        RotationalInvertDirectional,
        RotationalOnly,
        DirectionalOnly,
        InvertDirectionalOnly,
    }
}