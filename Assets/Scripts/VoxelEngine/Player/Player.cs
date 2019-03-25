using UnityEngine;
using VoxelEngine.UI;

namespace VoxelEngine.Player {

    public class Player : MonoBehaviour {
        int selected = -1;
        UIItemSlot[][] grid;

        void Start() {
            var hotbar = UIWindow.Create(8, 1, new Vector2(0.5f, 0));
            hotbar.SlotGrid(0, 0, 8, 1, "bar");
            grid = hotbar.GetGrid("bar");
        }

    }

}