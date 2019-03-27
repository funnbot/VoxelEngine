using UnityEngine;
using VoxelEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CodeTester : MonoBehaviour {
    public Texture2D InTex;
    public Texture2D OutTex;

    public Vector2Int axis;
    public float theta;

    object TestCode() {
        string o = "";

        OutTex = new Texture2D(InTex.width, InTex.height, TextureFormat.ARGB32, false, false);

        for (int x = 0; x < InTex.width; x++) {
            for (int y = 0; y < InTex.height; y++) {
                int x2 = Mathf.RoundToInt (Mathf.Cos(theta) * (x - axis.x) - Mathf.Sin(theta) * (y - axis.y) + axis.x);
                if (x2 < 0 || x2 >= InTex.width) continue;
                int y2 = Mathf.RoundToInt (Mathf.Sin(theta) * (x - axis.x) + Mathf.Cos(theta) * (y - axis.y) + axis.y);
                if (y2 < 0 || y2 >= InTex.height) continue;

                var col = InTex.GetPixel(x, y);
                OutTex.SetPixel(x2, y2, col);
            }
        }
        OutTex.Apply();

        return o;
    }

    bool runOnAwake = false;
    public void Run() {
        Debug.Log(TestCode());
    }
    void Awake() {
        if (runOnAwake) Run();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CodeTester))]
public class CodeTesterEditor : Editor {
    CodeTester c;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Run")) c.Run();
    }

    void OnEnable() {
        c = (CodeTester) target;
    }
}
#endif