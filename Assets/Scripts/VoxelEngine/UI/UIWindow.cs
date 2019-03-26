using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.UI {

    public class UIWindow : UIMonoBehaviour {
        static readonly int GridSize = 30;
        static readonly int GridSpacing = 10;

        static readonly int ButtonHeight = 25;

        public Vector2 position;
        public Vector2Int size;

        public delegate void Click(string buttonName);
        public event Click OnButtonClick;

        public delegate void SlotUpdate(string slotName, UIItemStack occupant);
        public event SlotUpdate OnSlotUpdate;

        public delegate void SlotGridUpdate(string slotName, Coord2 pos, UIItemStack occupant);
        public event SlotGridUpdate OnSlotGridUpdate;

        Dictionary<string, UIItemSlot> slots = new Dictionary<string, UIItemSlot>();
        Dictionary<string, UIItemSlot[][]> slotGrids = new Dictionary<string, UIItemSlot[][]>();

        public UIItemSlot Slot(float x, float y, string name) {
            var slot = UICanvas.UIInstantiate(UICanvas.ItemSlotFab).GetComponent<UIItemSlot>();
            PositionTransform(slot.transform, x, y, 1, 1);
            slot.OnUpdateOccupant += occupant => OnSlotUpdate?.Invoke(name, occupant);
            slots.Add(name, slot);
            return slot;
        }

        public UIItemSlot GetSlot(string name) {
            UIItemSlot slot = null;
            slots.TryGetValue(name, out slot);
            return slot;
        }

        public UIItemSlot[][] SlotGrid(float x, float y, int columns, int rows, string name) {
            var grid = new UIItemSlot[columns][];
            for (int xi = 0; xi < columns; xi++) {
                grid[xi] = new UIItemSlot[rows];
                for (int yi = 0; yi < rows; yi++) {
                    var slot = UICanvas.UIInstantiate(UICanvas.ItemSlotFab).GetComponent<UIItemSlot>();
                    PositionTransform(slot.transform, xi, yi, 1, 1);
                    grid[xi][yi] = slot;
                    slot.OnUpdateOccupant += occupant => OnSlotGridUpdate?.Invoke(name, new Coord2(xi, yi), occupant);
                }
            }
            slotGrids.Add(name, grid);
            return grid;
        }

        public UIItemSlot[][] GetGrid(string name) {
            UIItemSlot[][] grid = null;
            slotGrids.TryGetValue(name, out grid);
            return grid;
        }

        public void Button(float x, float y, string name, string text, int width = 2, int height = 1) {
            var button = UICanvas.UIInstantiate(UICanvas.ButtonFab).GetComponent<UIButton>();
            button.transform.SetParent(transform);
            button.transform.localPosition = TransformGridPosition(new Vector2(x, y));
            button.transform.sizeDelta = new Vector2(width * GridSize, height * ButtonHeight);
            button.OnClick += () => OnButtonClick?.Invoke(name);

            button.SetText(text);
        }

        public void Label(float x, float y, string text, Color color, int width = 3, int height = 1) {
            var label = UICanvas.UIInstantiate(UICanvas.LabelFab).GetComponent<TMPro.TMP_Text>();
            PositionTransform(label.rectTransform, x, y, width, height);

            label.SetText(text);
        }

        public void Open() {
            gameObject.SetActive(true);
        }

        public void Close() {
            gameObject.SetActive(false);
        }

        void PositionTransform(RectTransform rect, float x, float y, int width, int height) {
            rect.SetParent(transform);
            rect.localPosition = TransformGridPosition(new Vector2(x, y));
            rect.sizeDelta = new Vector2(width, height) * GridSize;
        }

        Vector2 TransformGridPosition(Vector2 pos) =>
            (pos * GridSize) - ((Vector2) size * (GridSize / 2f)) + (Vector2.one * (GridSize / 2f));

        public static UIWindow Create(int width, int height, Vector2 center) {
            var window = UICanvas.UIInstantiate(UICanvas.WindowFab).GetComponent<UIWindow>();

            window.transform.anchorMax = center;
            window.transform.anchorMin = center;

            var size = new Vector2Int(width, height);
            var sizeDelta = size * GridSize + (Vector2.one * GridSpacing);
            window.transform.sizeDelta = sizeDelta;
            window.size = size;

            var offset = (center * 2 - Vector2.one) * (sizeDelta / 2) * new Vector2(1, -1);
            Debug.Log(offset);
            //var offset = Vector2.zero;
            window.transform.anchoredPosition = offset;
            window.position = offset;

            return window;
        }
    }

}