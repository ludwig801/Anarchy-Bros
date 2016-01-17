using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScrollerBehavior))]
public class ScrollerBehaviorEditor : Editor
{
    ScrollerBehavior _script;

    void OnEnable()
    {
        _script = target as ScrollerBehavior;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = false;
        EditorGUILayout.IntField("Current Option", _script.CurrentOption);
        GUI.enabled = true;
    }
}
