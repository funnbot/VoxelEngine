﻿using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;

namespace VoxelEngine.Player {

    public class TerrainModifier : MonoBehaviour {
        public Vector3Int rotation;

        private VoxelWorld world;

        private Player player;

        BlockData air;
        string[] blocks = {
            "stone",
            "grass",
            "leaf",
            "wood",
            "hello",
            "circle"
        };

        int selected;

        void Start() {
            player = GetComponent<Player>();
            air = ResourceStore.Blocks["air"];
        }

        void Update() {
            world = WorldManager.ActiveWorld;
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
                    if (!Input.GetKey(KeyCode.LeftAlt)) {
                        var block = player?.activeStack?.item;
                        if (block != null) world.SetBlock(block, hit, rot, false);
                    } else {
                        world.SetBlock(air, hit, rot);
                    }
                }
            } else if (Input.GetMouseButtonDown(1)) {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out hit, 10f)) {
                    var block = world.GetBlock(hit, true);
                    //if (block is IInterfaceable);
                }
            }

            var cpos = (Coord2) Coord3.FloorToInt(transform.position).WorldToChunk();

            if (Input.GetKeyDown(KeyCode.L)) {
                if (!world.columns.ContainsKey(cpos)) {
                    world.LoadColumn(cpos);
                    Debug.Log("Loaded: " + cpos);
                }
            } else if (Input.GetKeyDown(KeyCode.B)) {
                if (world.columns.ContainsKey(cpos)) {
                    world.GetColumn(cpos).Build();
                    Debug.Log("Built: " + cpos);
                }
            } else if (Input.GetKeyDown(KeyCode.R)) {
                if (world.columns.ContainsKey(cpos)) {
                    world.GetColumn(cpos).QueueRender();
                    Debug.Log("Rendered: " + cpos);
                }
            } else if (Input.GetKeyDown(KeyCode.K)) {
                if (world.columns.ContainsKey(cpos)) {
                    world.DestroyColumn(cpos);
                    Debug.Log("Destroyed: " + cpos);
                }
            }
        }
    }

}