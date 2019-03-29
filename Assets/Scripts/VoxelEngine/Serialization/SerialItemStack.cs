using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

namespace VoxelEngine.Serialization {

    [MessagePackObject]
    public class SerialItemStack {
        [Key(0)]
        public string blockName;
        [Key(1)]
        public int count;
    }

}