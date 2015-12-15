using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapManager script = (MapManager)target;

        EditorGUILayout.LabelField("Is Targeting", script.Targeting.ToString());
    }
}
