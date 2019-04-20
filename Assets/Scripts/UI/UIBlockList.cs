using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Data;

namespace VoxelEngine.UI {

    public class UIBlockList : UIMonoBehaviour {
        public Transform Content;
        
        public delegate void Clicked(BlockData data);
        public event Clicked OnClicked;

        void Start() {
            foreach (BlockData block in ResourceStore.Blocks) {
                var slot = UICanvas.UIInstantiate(UICanvas.ItemSlotFab).GetComponent<RectTransform>();
                slot.transform.SetParent(Content);
                var stack = UICanvas.UIInstantiate<UIItemStack>(UICanvas.ItemStackFab);
                stack.transform.SetParent(slot.transform);
                stack.parentList = this;
                stack.SetStackData(block, 1);
            }
        }

        public void Click(BlockData data) {
            OnClicked?.Invoke(data);
        }
    }

}