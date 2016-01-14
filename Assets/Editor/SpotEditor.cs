using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spot))]
[CanEditMultipleObjects]
public class SpotEditor : Editor
{
    Spot _script;
    bool _occupied;

    void OnEnable()
    {
        _script = (Spot)target;
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
