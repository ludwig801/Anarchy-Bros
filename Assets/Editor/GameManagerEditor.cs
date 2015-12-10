using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager script = (GameManager)target;

        EditorGUILayout.LabelField("Current State", script.State);
    }
}