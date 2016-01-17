using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapSpot))]
[CanEditMultipleObjects]
public class SpotEditor : Editor
{
    MapSpot _script;
    bool _occupied;

    void OnEnable()
    {
        _script = (MapSpot)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.Toggle("Occupied", _script.Occupied);
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
}
