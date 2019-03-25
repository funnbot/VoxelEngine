using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoxelEngine.Data;

namespace VoxelEngine.UI {

    public class UIItemStack : UIMonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler {
        public Text CountText;
        public Text NameText;
        public Image Icon;

        [Space]
        public BlockData item;
        public int count;

        private UIItemSlot slot;

        public void SetStackData(BlockData item, int count) {
            this.item = item;
            this.count = count;

            SetText(CountText, count.ToString());
            SetText(NameText, item.blockName);
        }

        public void PlacedInSlot(UIItemSlot slot) {
            this.slot = slot;
        }

        public void RevertToSlot() {
            slot?.PlaceInSlot(this);
        }

        public void OnDrag(PointerEventData eventData) {
            transform.position = eventData.position;
        }

        public override void OnDragBegin() {
            slot?.PickupFromSlot();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Icon.color = new Color(1, 1, 1, 0.8f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            Icon.color = new Color(1, 1, 1, 1);
        }
    }
}