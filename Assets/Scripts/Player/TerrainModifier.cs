using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Internal;
using VoxelEngine.UI;

namespace VoxelEngine.Player {

    public class TerrainModifier : MonoBehaviour {
        public Vector3Int rotation;
        private VoxelWorld world;
        public PlayerSpawner controller;
        
        public UIBlockList blockList;
        private BlockData selected;

        void Start() {
            blockList.OnClicked += OnClick;
            blockList.Disable();
        }

        void OnClick(BlockData data) {
            selected = data;
        }

        void Update() {
            world = WorldManager.ActiveWorld;

            if (controller.locked && Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out hit, 10f)) {

                    if (hit.transform.CompareTag("Chunk")) {
                        if (!Input.GetKey(KeyCode.LeftAlt)) {
                            Block placed;
                            var chunk = world.PlaceBlock(hit, selected, out placed, false);
                            chunk.blocks.GetCustomBlock<RotatedBlock>(placed, b => b.SetRotation(rotation), true);
                        } else {
                            world.PlaceBlock(hit, null, true);
                        }
                    } else {
                        if (!Input.GetKey(KeyCode.LeftAlt)) {
                            Block placed;
                            var chunk = world.PlaceBlock(hit, selected, out placed, false);
                            chunk.blocks.GetCustomBlock<RotatedBlock>(placed, b => b.SetRotation(rotation), true);
                        } else {
                            Destroy(hit.transform.gameObject);
                        }
                    }

                }
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                if (!controller.locked) {
                    controller.Lock();
                    blockList.Disable();
                } else {
                    controller.Unlock();
                    blockList.Enable();
                }
            }
        }
    }

}