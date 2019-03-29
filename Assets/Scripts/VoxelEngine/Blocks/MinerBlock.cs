using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using VoxelEngine.Data;
using VoxelEngine.Interfaces;
using VoxelEngine.Inventory;
using VoxelEngine.Serialization;
using VoxelEngine.UI;

namespace VoxelEngine.Blocks {

    [MessagePackObject]
    public class MinerBlock : Block, IInterfaceable, IUpdateable, IPlaceHandler {
        public MinerBlock(Block clone) : base(clone) { }
        public MinerBlock() { }

        [Key(2)]
        public bool mining;
        [Key(3)]
        public Coord3 miningLocation;

        void IUpdateable.OnTick() {
            if (!mining) return;
            chunk.SetBlock(null, miningLocation.WorldToBlock(chunk.worldPosition), Coord3.zero, true);

            miningLocation.z++;
            if (miningLocation.z > position.z + 2) {
                miningLocation.z = position.z - 2;
                miningLocation.x++;
            }
            if (miningLocation.x > position.x + 2) {
                miningLocation.x = position.x - 2;
                miningLocation.y--;
            }
            if (miningLocation.y <= 0) mining = false;
        }

        private UIItemStack[, ] inv;
        private ItemStackGrid inventory;

        static UIWindow window;
        void IInterfaceable.BuildGUI() {
            if (window != null) return;
            window = UIWindow.Create(15, 10, new Vector2(0.5f, 0.5f));
            window.SlotGrid(0, 5, 15, 5, "inventory");
            window.Button(7.5f, 7, "mine", "Mine!");
        }

        void IInterfaceable.OpenGUI() {
            if (inv == null) inv = new UIItemStack[15, 5];
            window.OnButtonClick += OnButtonClick;

            var grid = window.GetGrid("inventory");
            for (int x = 0; x < 15; x++) {
                for (int y = 0; y < 5; y++) {
                    if (inv[x, y] != null) grid[x][y].PlaceInSlot(inv[x, y]);
                    inv[x, y]?.gameObject.SetActive(true);
                }
            }

            window.Open();
        }

        void IInterfaceable.CloseGUI() {
            window.OnButtonClick -= OnButtonClick;
            window.Close();

            var grid = window.GetGrid("inventory");
            for (int x = 0; x < 15; x++) {
                for (int y = 0; y < 5; y++) {
                    inv[x, y] = grid[x][y].occupant;
                    grid[x][y].PickupFromSlot();
                    inv[x, y]?.gameObject.SetActive(false);
                }
            }
        }

        void OnButtonClick(string name) {
            if (name == "mine") mining = !mining;
        }

        void IPlaceHandler.OnPlace() {
            Debug.Log("Place");
            miningLocation = position - new Coord3(2, 1, 2);
        }
    }

}