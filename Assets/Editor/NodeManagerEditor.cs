using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(GraphManager))]
public class GraphManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GraphManager script = (GraphManager)target;

        EditorGUILayout.LabelField("Is Targeting", script.Targeting.ToString());
    }
}
