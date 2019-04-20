using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIButton))]
public class ButtonLoadScene : MonoBehaviour {
    public string ToLoad;
    private UIButton button;

    void Start() {
        button = GetComponent<UIButton>();
        button.OnClick += OnClick;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    void OnClick() {
        SceneManager.LoadScene(ToLoad);
    }
}