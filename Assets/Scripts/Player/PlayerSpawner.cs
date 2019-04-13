using UnityEngine;
using Controller = UnityStandardAssets.Characters.FirstPerson.FirstPersonController;

namespace VoxelEngine.Player {

    public class PlayerSpawner : MonoBehaviour {
        void Start() {

            WorldManager.ActiveWorld.OnSpawnLoad += OnSpawnLoad;
        }

        void OnSpawnLoad() {
            RaycastHit hit;
            Vector3 origin = new Vector3(8, 400, 8);
            Ray ray = new Ray(origin, Vector3.down);
            var cc = GetComponent<CharacterController>();
            var c = GetComponent<Controller>();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                cc.enabled = false;
                c.enabled = false;

                cc.transform.position = hit.point + Vector3.up * 3;

                cc.enabled = true;
                c.enabled = true;
            }
        }
    }

}