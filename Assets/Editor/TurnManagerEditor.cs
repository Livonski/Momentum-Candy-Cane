using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TurnManager))]
public class TurnManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default convertor

        TurnManager script = (TurnManager)target;

        if (GUILayout.Button("Next turn"))
            script.ExecuteTurnEditor();

        if (GUILayout.Button("Next Sub-turn"))
            script.NextSubturn();
    }
}
