using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Texture Index")]
public class TextureIndex : ScriptableObject {
    public int index;
    public Texture2D texture;
}