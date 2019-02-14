using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraController : MonoBehaviour {
    public float moveSpeed;
    public float fastMoveSpeed;
    public float lookSpeed;

    Vector2 mouse;
    Vector3 input;

    void Start() {
        mouse = new Vector2(transform.localEulerAngles.x, transform.localEulerAngles.y);
    }

    void Update() {
        var speed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
        var inputY = (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0);
        mouse += new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X")) * lookSpeed;
        input = new Vector3(Input.GetAxisRaw("Horizontal"), inputY, Input.GetAxisRaw("Vertical")) * speed;
    }

    void LateUpdate() {
        transform.localEulerAngles = mouse;
        transform.Translate(input * Time.deltaTime);
    }
}