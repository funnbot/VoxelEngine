using UnityEngine;
using Controller = UnityStandardAssets.Characters.FirstPerson.FirstPersonController;

// Delete this
namespace VoxelEngine.Player {

    public class PlayerSpawner : MonoBehaviour {
        Controller contr;
        CharacterController ccontr;

        public bool locked;

        void Start() {
            ccontr = GetComponent<CharacterController>();
            contr = GetComponent<Controller>();
            Lock();
            if (ccontr != null) {
                WorldManager.ActiveWorld.OnSpawnLoad += OnSpawnLoad;
                ccontr.enabled = false;
                contr.enabled = false;
            }
        }

        void OnSpawnLoad() {
            RaycastHit hit;
            Vector3 origin = new Vector3(8, 400, 8);
            Ray ray = new Ray(origin, Vector3.down);
            if (Physics.Raycast(ray, out hit)) {
                ccontr.enabled = false;
                contr.enabled = false;

                ccontr.transform.position = hit.point + Vector3.up * 3;

                ccontr.enabled = true;
                contr.enabled = true;
            }
        }

        public void Lock() {
            if (ccontr != null) {
                ccontr.enabled = true;
                contr.enabled = true;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            locked = true;
        }

        public void Unlock() {
            if (ccontr != null) {
                ccontr.enabled = false;
                contr.enabled = false;
            }
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            locked = false;
        }
    }

}