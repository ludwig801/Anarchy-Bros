using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    Map _script;

    public void OnEnable()
    {
        _script = target as Map;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = false;
        EditorGUILayout.Toggle("Is Targeting", _script.Targeting);
        GUI.enabled = true;
    }
}
