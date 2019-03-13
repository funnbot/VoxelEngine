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