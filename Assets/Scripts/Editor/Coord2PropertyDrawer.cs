using UnityEditor;
using UnityEngine;
using VoxelEngine;

[CustomPropertyDrawer(typeof(Coord2))]
public class Coord2PropertyDrawer : PropertyDrawer
{
    SerializedProperty X, Y;
    string name;
    bool cache = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!cache)
        {
            //get the name before it's gone
            name = property.displayName;

            //get the X and Y values
            property.Next(true);
            X = property.Copy();
            property.Next(true);
            Y = property.Copy();

            cache = true;
        }

        Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));

        //Check if there is enough space to put the name on the same line (to save space)
        if (position.height > 16f)
        {
            position.height = 16f;
            EditorGUI.indentLevel += 1;
            contentPosition = EditorGUI.IndentedRect(position);
            contentPosition.y += 18f;
        }

        float half = contentPosition.width / 2;
        GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);

        //show the X and Y from the point
        EditorGUIUtility.labelWidth = 14f;
        contentPosition.width *= 0.5f;
        EditorGUI.indentLevel = 0;

        // Begin/end property & change check make each field
        // behave correctly when multi-object editing.
        EditorGUI.BeginProperty(contentPosition, label, X);
        {
            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUI.IntField(contentPosition, new GUIContent("X"), X.intValue);
            if (EditorGUI.EndChangeCheck())
                X.intValue = newVal;
        }
        EditorGUI.EndProperty();

        contentPosition.x += half;

        EditorGUI.BeginProperty(contentPosition, label, Y);
        {
            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUI.IntField(contentPosition, new GUIContent("Y"), Y.intValue);
            if (EditorGUI.EndChangeCheck())
                Y.intValue = newVal;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return Screen.width < 333 ? (16f + 18f) : 16f;
    }
}