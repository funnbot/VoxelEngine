﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoxelEngine.Data;

namespace VoxelEngine.UI {

    [RequireComponent(typeof(Image))]
    public class UIItemSlot : UIMonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
        [System.NonSerialized]
        public UIItemStack occupant;
        public bool locked;

        public delegate void UpdateOccupant(UIItemStack occupant);
        public event UpdateOccupant OnUpdateOccupant;

        private Image panel;

        protected override void AwakeImpl() {
            panel = GetComponent<Image>();
            if (occupant != null) PlaceInSlot(occupant);
        }

        public void SetColor(Color color) {
            panel.color = color;
        }

        public void PlaceInSlot(UIItemStack stack) {
            occupant = stack;
            stack.transform.SetParent(transform);
            stack.transform.localPosition = Vector2.zero;
            stack.PlacedInSlot(this);
            OnUpdateOccupant?.Invoke(occupant);
        }

        public void PickupFromSlot() {
            if (occupant == null) return;
            occupant.transform.SetParent(UICanvas.Transform);
            occupant = null;
            OnUpdateOccupant?.Invoke(null);
        }

        // When a dragged item is dropped on this slot
        void IDropHandler.OnDrop(PointerEventData eventData) {
            if (eventData.pointerDrag == null) return;

            var stack = eventData.pointerDrag.GetComponent<UIItemStack>();
            if (stack == null) return;

            if (!locked || occupant == null) PlaceInSlot(stack);
            else stack.RevertToSlot();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            var col = panel.color;
            col.a = 0.8f;
            panel.color = col;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            var col = panel.color;
            col.a = 1f;
            panel.color = col;
        }
    }

}