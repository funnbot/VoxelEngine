using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;

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
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    inGui.CloseGUI();
                    inGui = null;
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
                            if (block != null) world.SetBlock(block, hit, rot, false);
                        } else {
                            world.SetBlock(air, hit, rot);
                        }
                    } else {
                        int y = -Mathf.RoundToInt(transform.localEulerAngles.y / 90f);
                        var rot = new Coord3(0, y, 0);
                        if (!Input.GetKey(KeyCode.LeftAlt)) {
                            var block = player?.activeStack?.item;
                            if (block != null) world.SetBlock(block, hit, rot, false);
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
                    }

                }
            }
        }
    }

}