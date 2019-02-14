using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

public class TerrainModifier : MonoBehaviour {
    public VoxelWorld world;

    BlockData air;
    string[] blocks = {
        "stone",
        "grass",
        "leaf",
        "wood"
    };

    int selected;

    void Start() {
        air = ResourceStore.Blocks["air"];
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (++selected >= blocks.Length) selected = 0;
            Debug.Log("Selected: " + blocks[selected]);
        } else if (Input.GetKeyDown(KeyCode.Q)) {
            if (--selected < 0) selected = blocks.Length - 1;
            Debug.Log("Selected: " + blocks[selected]);
        }

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, 10f)) {
                if (!Input.GetKey(KeyCode.LeftAlt)) {
                    var block = ResourceStore.Blocks[blocks[selected]];
                    world.SetBlock(hit, new Block(block), false);
                } else {
                    world.SetBlock(hit, new Block(air));
                }
            }
        }
    }
}