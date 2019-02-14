using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    [CreateAssetMenu(fileName = "Block Data")]
    public class BlockData : ScriptableObject {
        public bool transparent;
        public BlockMeshType meshType;
        public Mesh customMesh;
        public string behaviourType;

        public Vector2Int[] FaceTiling = new Vector2Int[6];

        public SpawnData spawnData;
        public class SpawnData {

        }
    }

    public enum BlockMeshType {
        Cube,
        Decal,
        Air,
        Custom
    }

}