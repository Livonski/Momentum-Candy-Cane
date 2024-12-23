using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilemapReader))]
public class TilemapReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default convertor

        TilemapReader script = (TilemapReader)target;

        if (GUILayout.Button("Save tilemap"))
            script.ExportTilemapToJson();
    }
}
