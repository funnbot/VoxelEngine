using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.UI {

    public class UICanvas : SingletonMonoBehaviour<UICanvas> {
        [SerializeField]
        private Canvas _canvas;
        public static Canvas Canvas { get => Instance._canvas; }

        public static Transform Transform { get => Instance._canvas.transform; }

        [SerializeField]
        private GameObject _itemStackFab;
        public static GameObject ItemStackFab { get => Instance._itemStackFab; }

        [SerializeField]
        private GameObject _itemSlotFab;
        public static GameObject ItemSlotFab { get => Instance._itemSlotFab; }

        [SerializeField]
        private GameObject _windowFab;
        public static GameObject WindowFab { get => Instance._windowFab; }

        [SerializeField]
        private GameObject _buttonFab;
        public static GameObject ButtonFab { get => Instance._buttonFab; }

        [SerializeField]
        private GameObject _labelFab;
        public static GameObject LabelFab { get => Instance._labelFab; }

        public static GameObject UIInstantiate(GameObject prefab) {
            return Instantiate(prefab, Transform);
        }

        void Start() {
            var window = UIWindow.Create(14, 6, new Vector2(0.5f, 0.5f));

            window.Label(6.5f, 1, "Label Me.", Color.white);
            window.Button(6.5f, 2, "button1", "Click Me!", 3);
            window.Button(6.5f, 4, "button2", "Click Me!", 3);
            window.SlotGrid(0, 5, 3, 3, "crafting");

            window.OnClick += name => Debug.Log(name);

            window.Close();
        }
    }

}