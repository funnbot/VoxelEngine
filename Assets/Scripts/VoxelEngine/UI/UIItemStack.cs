using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoxelEngine.Data;

namespace VoxelEngine.UI {

    public class UIItemStack : UIMonoBehaviour, IDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
        public Text CountText;
        public Text NameText;
        public Image Icon;
        
        [Space]
        public BlockData item;
        public int count;

        public void Init(BlockData item, int count) {
            this.item = item;
            this.count = count;
            
            SetText(CountText, count.ToString());
            SetText(NameText, item.blockName);
        }

        public void OnDrag(PointerEventData eventData) {
            if (transform.parent != null) {
                var slot = transform.parent.GetComponent<UIItemSlot>();
                slot?.Pickup();
            }
            transform.position = eventData.position;
        }

        public void OnDrop(PointerEventData eventData) {
            Debug.Log("Drop");
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Debug.Log("Enter");
            Icon.color = new Color(1, 1, 1, 0.9f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            Icon.color = new Color(1, 1, 1, 1);
        }
    }
}