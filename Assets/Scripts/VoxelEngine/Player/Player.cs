using UnityEngine;
using VoxelEngine.UI;
using VoxelEngine.Inventory;

namespace VoxelEngine.Player {

    public class Player : MonoBehaviour {
        public ColliderBoundsOutliner BoundsOutliner;
        public PlayerChunkLoader ChunkLoader;
        public PlayerSpawner Spawner;
        public TerrainModifier Terrain;
        public FreeCameraController CameraController;

        public UIItemStack activeStack;
        int selected = 0;
        UIItemSlot[][] grid;

        static Color selectedColor = Color.yellow;

        void Start() {
            

            Cursor.lockState = CursorLockMode.Locked;

            var hotbar = UIWindow.Create(8, 1, new Vector2(0.5f, 0));
            grid = hotbar.SlotGrid(0, 0, 8, 1, "bar");

            SetSlotTo("virus", 0);
            SetSlotTo("dirt", 1);
            SetSlotTo("grass", 2);
            SetSlotTo("wood", 3);
            SetSlotTo("pipe", 4);
            SetSlotTo("iron_ore", 5);
            SetSlotTo("ribit", 6);
            SetSlotTo("miner", 7);

            Select(0);
        }

        void SetSlotTo(string name, int i) {
            var virus = ResourceStore.Blocks[name];
            var stack = UICanvas.UIInstantiate(UICanvas.ItemStackFab).GetComponent<UIItemStack>();
            stack.SetStackData(virus, 1);
            grid[i][0].PlaceInSlot(stack);
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Select(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) Select(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) Select(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) Select(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) Select(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) Select(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7)) Select(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8)) Select(7);
        }

        void Select(int slotNum) {
            grid[selected][0].SetColor(Color.white);
            grid[slotNum][0].SetColor(selectedColor);
            selected = slotNum;
            activeStack = grid[selected][0].occupant;
        }
    }

}