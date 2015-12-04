using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(Edge))]
public class EdgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Edge script = target as Edge;

        EditorGUILayout.ObjectField("Vertex A", script.A, typeof(Node), true, null);
        EditorGUILayout.ObjectField("Vertex B", script.B, typeof(Node), true, null);
    }
}