using UnityEngine;
using VoxelEngine.Blocks;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Internal;

namespace VoxelEngine.Player {

    public class TerrainModifier : MonoBehaviour {
        public Vector3Int rotation;
        private VoxelWorld world;
        
        BlockData selected;

        void Start() {
            selected = ResourceStore.Blocks["miner"];
        }

        void Update() {
            world = WorldManager.ActiveWorld;

            if (Input.GetMouseButtonDown(0)) {
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
        }
    }

}