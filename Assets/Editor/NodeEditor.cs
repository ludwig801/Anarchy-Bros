using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Node script = (Node)target;

        EditorGUILayout.Toggle("Occupied", script.Occupied);
    }
}
