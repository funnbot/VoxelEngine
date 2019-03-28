using UnityEngine;

namespace VoxelEngine.Player {

    public class FreeCameraController : MonoBehaviour {
        public float moveSpeed;
        public float fastMoveSpeed;
        public float lookSpeed;

        Vector2 mouse;
        Vector3 input;
        bool locked;

        void Start() {
            mouse = new Vector2(transform.localEulerAngles.x, transform.localEulerAngles.y);
        }

        void Update() {
            if (locked) return;
            var speed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
            // var inputY = (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0);
            mouse += new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X")) * lookSpeed;
            input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * speed;
        }

        void LateUpdate() {
            if (locked) return;
            transform.localEulerAngles = mouse;
            transform.Translate(input * Time.deltaTime);
        }

        public void Lock() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            locked = false;
        }

        public void Unlock() {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            locked = true;
        }
    }

}