using UnityEngine;
using VoxelEngine.Interfaces;
using VoxelEngine.UI;

namespace VoxelEngine.Blocks {

    public class HelloBlock : Block, IInterfaceable {
        public HelloBlock(Block block) : base(block) { }
        
        static UIWindow window;
        public void OpenGUI() {
            //if (window == null) window = UIWindow.Create(0, 0, );

            window.Open();
        }

        public void CloseGUI() {
            window.Close();
        }
    }

}