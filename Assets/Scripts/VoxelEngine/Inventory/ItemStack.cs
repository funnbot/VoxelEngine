using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.UI;

namespace VoxelEngine.Inventory {

    [MessagePackObject]
    public class ItemStack {
        [Key(0)]
        public string itemName;
        [Key(1)]
        public int count;

        [IgnoreMember]
        public BlockData item;
    }

}