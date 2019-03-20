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
    }

}