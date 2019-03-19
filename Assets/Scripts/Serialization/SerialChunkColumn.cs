using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

[System.Serializable]
public class SerialChunkColumn {
    [SerializeField]
    public Block[][][][] blocks;
}