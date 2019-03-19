using UnityEngine;
using VoxelEngine.Interfaces;

namespace VoxelEngine.Blocks {

    public class HelloBlock : Block, IInterfaceable {
        public HelloBlock(Block block) : base(block) { }

        public bool DrawGUI() {
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 200, 100), "Test Menu");

            if (GUI.Button(new Rect(100, 100, 150, 100), "I am a button")) {
                return false;
            }
            if (Input.GetKey(KeyCode.Escape)) return false;
            return true;
        }
    }

}