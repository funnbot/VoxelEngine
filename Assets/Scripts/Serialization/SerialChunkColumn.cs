using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
using MessagePack;

[System.Serializable, MessagePackObject]
public class SerialChunkColumn {
    [Key(0)]
    public Block[][][][] blocks;
}