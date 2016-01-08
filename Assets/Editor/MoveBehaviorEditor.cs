using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveBehavior))]
public class MoveBehaviorEditor : Editor
{
    MoveBehavior _script;

    void OnEnable()
    {
        _script = target as MoveBehavior;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = false;
        EditorGUILayout.Toggle("Moving", _script.IsMoving);
        EditorGUILayout.Toggle("Spot", _script.HasCurrentSpot);
        EditorGUILayout.Toggle("Edge", _script.HasCurrentEdge);
        GUI.enabled = true;
    }
}
