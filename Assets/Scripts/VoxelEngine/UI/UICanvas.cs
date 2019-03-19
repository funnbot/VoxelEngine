using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.UI {

    public class UICanvas : SingletonMonobehaviour<UICanvas> {
        [SerializeField]
        private Canvas _canvas;
        public static Canvas Canvas { get => Instance._canvas; }

    }

}