﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

public class TerrainModifier : MonoBehaviour {
    public VoxelWorld world;

    public Vector3Int rotation;

    BlockData air;
    string[] blocks = {
        "stone",
        "grass",
        "leaf",
        "wood",
        "hello"
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
                int y = -Mathf.RoundToInt(transform.localEulerAngles.y / 90f);
                var rot = new Coord3(1, y, 0);
                // rot = (Coord3)rotation;
                Debug.Log(rot);
                if (!Input.GetKey(KeyCode.LeftAlt)) {
                    var block = ResourceStore.Blocks[blocks[selected]];

                    world.SetBlock(block, hit, rot, false);
                } else {
                    world.SetBlock(air, hit, rot);
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, 10f)) {
                var block = world.GetBlock(hit, true);
                if (block is IInterfaceable)
                    activeGUI = (IInterfaceable) block;
            }
        }
    }

    IInterfaceable activeGUI;

    void OnGUI() {
        if (activeGUI != null) {
            if (!activeGUI.DrawGUI())
                activeGUI = null;
        }
    }
}