using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

namespace VoxelEngine.Serialization {

    [System.Serializable]
    public class SerialItemStack {
        public string blockName;
        public int count;
    }

}