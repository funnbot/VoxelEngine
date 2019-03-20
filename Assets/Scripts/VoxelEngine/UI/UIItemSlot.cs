using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoxelEngine.Data;

namespace VoxelEngine.UI {

    public class UIItemSlot : UIMonoBehaviour, IDropHandler {
        [System.NonSerialized]
        public UIItemStack occupant;

        void Start() {
            if (occupant != null) Place(occupant);
        }

        void Update() {

        }

        public bool Place(UIItemStack stack) {
            if (occupant != null) return false;
            occupant = stack;
            stack.transform.SetParent(transform);
            stack.transform.localPosition = Vector2.zero;
            return true;
        }

        public UIItemStack Pickup() {
            var stack = occupant;
            occupant = null;
            stack.transform.SetParent(UICanvas.Transform);
            return stack;
        }

        public void OnDrop(PointerEventData eventData) {
            Debug.Log("Pointer up");
            if (eventData.pointerDrag == null) return;
            Debug.Log("drop");
            var stack = eventData.pointerDrag.GetComponent<UIItemStack>();
            if (stack == null) return;

            Place(stack);
        }
    }

}