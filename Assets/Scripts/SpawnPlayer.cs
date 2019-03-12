using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
using Controller = UnityStandardAssets.Characters.FirstPerson.FirstPersonController;

public class SpawnPlayer : MonoBehaviour {
    public VoxelWorld world;

    void Start() {
        world.OnSpawnLoad += OnSpawnLoad;
    }

    void OnSpawnLoad() {
        RaycastHit hit;
        Ray ray = new Ray(Vector3.up * 100, Vector3.down);
        if (Physics.Raycast(ray, out hit)) {
            var cc = GetComponent<CharacterController>();
            cc.enabled = false;
            cc.transform.position = hit.point + Vector3.one * 3;
            cc.enabled = true;
        }
        Debug.Log(hit.point);
    }

}