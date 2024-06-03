using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShiresController))]
public class ShiresControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShiresController shiresController = (ShiresController)target;

        if (GUILayout.Button("Reset PlayerPrefs"))
        {
            shiresController.ClearPlayerPrefs();
        }
    }
}
