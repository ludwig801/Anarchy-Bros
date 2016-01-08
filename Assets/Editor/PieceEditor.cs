using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Piece))]
public class PieceEditor : Editor
{
    Piece _script;

    void OnEnable()
    {
        _script = target as Piece;
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", _script, typeof(Piece), false);
        GUI.enabled = true;
        _script.MaxHealth = EditorGUILayout.IntField("Full Health", _script.MaxHealth);
        GUI.enabled = false;
        EditorGUILayout.IntSlider("Health", _script.Health, 0, _script.MaxHealth);
        GUI.enabled = true;
        _script.DeathSpeed = EditorGUILayout.Slider("Death Time", _script.DeathSpeed, 0f, 2f);
        _script.TargetTag = (Tags.Tag)EditorGUILayout.EnumPopup("Target Tag", _script.TargetTag);
        GUI.enabled = false;
        EditorGUILayout.Toggle("Alive", _script.Alive);
        EditorGUILayout.Toggle("Is Attacking", _script.IsAttacking);
        GUI.enabled = true;
    }
}
