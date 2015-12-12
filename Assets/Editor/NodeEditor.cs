using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(Spot))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Spot script = (Spot)target;

        EditorGUILayout.Toggle("Occupied", script.Occupied);
    }
}
