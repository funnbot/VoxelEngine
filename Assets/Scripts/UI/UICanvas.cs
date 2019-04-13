using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Internal;

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

        public RectTransform rectTransform;
        public static RectTransform RectTransform { get => Instance.rectTransform; }

        public static Transform Transform { get => Instance.canvas.transform; }

        public static GameObject UIInstantiate(GameObject prefab) {
            return Instantiate(prefab, Transform);
        }

        public static T UIInstantiate<T>(GameObject prefab) where T : MonoBehaviour {
            return Instantiate(prefab, Transform).GetComponent<T>();
        }
    }

}