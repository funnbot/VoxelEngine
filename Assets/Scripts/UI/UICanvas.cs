using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : SingletonMonobehaviour<UICanvas> {
    [SerializeField]
    private Canvas _canvas;
    public static Canvas Canvas { get => Instance._canvas; }

    
}
