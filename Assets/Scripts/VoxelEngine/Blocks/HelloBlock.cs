using UnityEngine;
using VoxelEngine.Interfaces;
using VoxelEngine.UI;

namespace VoxelEngine.Blocks {

    public class HelloBlock : Block, IInterfaceable {
        public HelloBlock(Block block) : base(block) { }

        static UIWindow window;
        public void BuildGUI() {
            window = UIWindow.Create(10, 10, new Vector2(0.5f, 0.5f));
        }

        public void OpenGUI() {
            window.Open();
            
        }

        public void CloseGUI() {
            window.Close();
        }
    }

}