using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoxelEngine.Data;

namespace VoxelEngine.UI {

    public class UIItemStack : UIMonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler {
        public GameObject ItemStack_CubeFab;
        public GameObject ItemStack_IconFab;
        public Sprite DiamondSprite;

        public TMPro.TMP_Text CountText;
        public TMPro.TMP_Text NameText;

        // Cube
        GameObject CubeDisplay;
        RawImage CubeTop;
        RawImage CubeRight;
        RawImage CubeLeft;

        // Icon
        RawImage IconDisplay;

        [Space]
        public BlockData item;
        public int count;

        private UIItemSlot slot;

        public void SetStackData(BlockData item, int count) {
            this.item = item;
            this.count = count;

            SetText(CountText, count.ToString());
            SetText(NameText, item.blockName);

            if (item.blockType == BlockType.Cube) {
                CubeDisplay = Instantiate(ItemStack_CubeFab, transform);
                CubeDisplay.transform.SetAsFirstSibling();
                CubeTop = CubeDisplay.transform.Find("Top").GetComponent<RawImage>();
                CubeLeft = CubeDisplay.transform.Find("Left").GetComponent<RawImage>();
                CubeRight = CubeDisplay.transform.Find("Right").GetComponent<RawImage>();

                CubeTop.texture = item.textures[2]?.texture;
                CubeLeft.texture = item.textures[5]?.texture;
                CubeRight.texture = item.textures[0]?.texture;
            } else {
                IconDisplay = Instantiate(ItemStack_IconFab, transform).GetComponent<RawImage>();
                IconDisplay.texture = item.icon ?? item.textures[0]?.texture;
            }
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
            //Icon.color = new Color(1, 1, 1, 0.8f);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            //Icon.color = new Color(1, 1, 1, 1);
        }
    }
}