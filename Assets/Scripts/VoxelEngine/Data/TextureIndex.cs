using UnityEngine;

namespace VoxelEngine.Data {

    [CreateAssetMenu(fileName = "Texture Index")]
    public class TextureIndex : ScriptableObject {
        //[HideInInspector]
        public int index;
        public Texture2D texture;
    }

}