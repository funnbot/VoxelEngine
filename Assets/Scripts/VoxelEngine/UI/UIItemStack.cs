using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoxelEngine.Data;

namespace VoxelEngine.UI {

    public class UIItemStack : UIMonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler {
        public TMPro.TMP_Text CountText;
        public TMPro.TMP_Text NameText;
        public Image Icon;

        [Space]
        public ItemStack data;
        public BlockData item;
        public int count;

        private UIItemSlot slot;

        public void SetStackData(BlockData item, int count) {
            this.item = item;
            this.count = count;

            SetText(CountText, count.ToString());
            SetText(NameText, item.blockName);
            SetImage(Icon, item.icon);
        }

        public void PlacedInSlot(UIItemSlot slot) {
            this.slot = slot;
        }

        public void RevertToSlot() {
            slot?.PlaceInSlot(this);
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            transform.position = eventData.position;
        }

        public override void OnDragBegin() {
            if (slot != null && !slot.locked) {
                slot.PickupFromSlot();
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            Icon.color = new Color(1, 1, 1, 0.8f);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            Icon.color = new Color(1, 1, 1, 1);
        }
    }
}