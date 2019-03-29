using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.UI {

    public class UICanvas : SingletonMonoBehaviour<UICanvas> {
        public Canvas canvas;
        public static Canvas Canvas { get => Instance.canvas; }

        public GameObject itemStackFab;
        public static GameObject ItemStackFab { get => Instance.itemStackFab; }

        public GameObject itemSlotFab;
        public static GameObject ItemSlotFab { get => Instance.itemSlotFab; }

        public GameObject windowFab;
        public static GameObject WindowFab { get => Instance.windowFab; }

        public GameObject buttonFab;
        public static GameObject ButtonFab { get => Instance.buttonFab; }

        public GameObject labelFab;
        public static GameObject LabelFab { get => Instance.labelFab; }

        public static Transform Transform { get => Instance.canvas.transform; }

        public static GameObject UIInstantiate(GameObject prefab) {
            return Instantiate(prefab, Transform);
        }

        public static T UIInstantiate<T>(GameObject prefab) where T : MonoBehaviour {
            return Instantiate(prefab, Transform).GetComponent<T>();
        }

        void Start() {
            var window = UIWindow.Create(14, 6, new Vector2(0.5f, 0.5f));

            window.Label(6.5f, 1, "Label Me.", Color.white);
            window.Button(6.5f, 2, "button1", "Click Me!", 3);
            window.Button(6.5f, 4, "button2", "Click Me!", 3);
            window.SlotGrid(0, 5, 3, 3, "crafting");

            window.OnButtonClick += name => Debug.Log(name);

            window.Close();
        }
    }

}