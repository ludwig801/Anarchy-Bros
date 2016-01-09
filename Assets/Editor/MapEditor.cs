using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class MapEditor : Editor
{
    MapManager _script;

    public void OnEnable()
    {
        _script = target as MapManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = false;
        EditorGUILayout.Toggle("Is Targeting", _script.Targeting);
        GUI.enabled = true;
    }
}
