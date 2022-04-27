using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MusicManager))]
public class MusicManagerEditor : Editor
{
    int index = 0;
    UnitType[] unitTypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().ToArray();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MusicManager manager = (MusicManager)target;

        index = EditorGUILayout.Popup("Simulate Unit:", index, (from t in unitTypes select t.ToString()).ToArray(), EditorStyles.popup);

        if (GUILayout.Button("Add Unit"))
        {
            manager.AddTheme(unitTypes[index]);
        }

        if (GUILayout.Button("Remove Unit"))
        {
            manager.RemoveTheme(unitTypes[index]);
        }
    }
}
