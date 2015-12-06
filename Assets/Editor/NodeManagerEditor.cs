using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(NodeManager))]
public class NodeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NodeManager script = (NodeManager)target;

        EditorGUILayout.LabelField("Is Targeting", script.Targeting.ToString());
    }
}
