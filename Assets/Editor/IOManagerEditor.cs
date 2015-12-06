using UnityEngine;
using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(IOManager))]
public class IOManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        IOManager script = (IOManager)target;

        if (GUILayout.Button("Rebuild"))
        {
            script.LoadGraph();
        }
    }
}
