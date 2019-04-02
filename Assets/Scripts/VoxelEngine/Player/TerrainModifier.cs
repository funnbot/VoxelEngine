using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Internal;

namespace VoxelEngine.Player {

    public class TerrainModifier : MonoBehaviour {
        public Vector3Int rotation;

        private VoxelWorld world;

        private Player player;

        IInterfaceable inGui;

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

            if (inGui != null) {
                if (Input.GetKeyDown(KeyCode.Tab)) {
                    inGui.CloseGUI();
                    inGui = null;
                    player.CameraController.Lock();
                }
            } else if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out hit, 10f)) {

                    if (hit.transform.CompareTag("Chunk")) {
                        int y = -Mathf.RoundToInt(transform.localEulerAngles.y / 90f);
                        var rot = new Coord3(0, y, 0);
                        if (!Input.GetKey(KeyCode.LeftAlt)) {
                            var block = player?.activeStack?.item;
                            if (block != null) {
                                Block placed;
                                var chunk = world.PlaceBlock(hit, block, out placed, false);
                                chunk.blocks.GetCustomBlock<RotatedBlock>(placed, b => b.SetRotation(rot));
                            }
                        } else {
                            world.PlaceBlock(hit, null, true);
                        }
                    } else {
                        int y = -Mathf.RoundToInt(transform.localEulerAngles.y / 90f);
                        var rot = new Coord3(0, y, 0);
                        if (!Input.GetKey(KeyCode.LeftAlt)) {
                            var block = player?.activeStack?.item;
                            if (block != null) {
                                Block placed;
                                var chunk = world.PlaceBlock(hit, block, out placed, false);
                                chunk.blocks.GetCustomBlock<RotatedBlock>(placed, b => b.SetRotation(rot));
                            }
                        } else {
                            Destroy(hit.transform.gameObject);
                        }
                    }

                }
            } else if (Input.GetMouseButtonDown(1)) {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out hit, 10f)) {
                    var block = world.GetBlock(hit, true);
                    inGui = block as IInterfaceable;
                    if (inGui != null) {
                        inGui.BuildGUI();
                        inGui.OpenGUI();
                        player.CameraController.Unlock();
                    }

                }
            }
        }
    }

}