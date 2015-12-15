using UnityEngine;
using UnityEditor;
using AnarchyBros;

[CustomEditor(typeof(EditBehavior))]
[CanEditMultipleObjects]
public class PieceBehaviorEditor : Editor
{
    SerializedProperty _script;
    SerializedProperty _colorDefault, _colorMouseOver;
    SerializedProperty _animationsSpeed, _scaleOnMouseOver, _scaleOnClick;
    SerializedProperty _clickScale, _mouseOverScale;

    void OnEnable()
    {
        _script = serializedObject.FindProperty("m_Script");
        _animationsSpeed = serializedObject.FindProperty("AnimationsSpeed");
        _colorDefault = serializedObject.FindProperty("ColorDefault");
        _colorMouseOver = serializedObject.FindProperty("ColorMouseOver");
        _scaleOnMouseOver = serializedObject.FindProperty("ScaleOnMouseOver");
        _scaleOnClick = serializedObject.FindProperty("ScaleOnClick");
        _mouseOverScale = serializedObject.FindProperty("MouseOverScale");
        _clickScale = serializedObject.FindProperty("ClickScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(_script, false);
        GUI.enabled = true;

        EditorGUILayout.Slider(_animationsSpeed, 0, 10, new GUIContent("Speed"));

        EditorGUILayout.PropertyField(_colorDefault, new GUIContent("Color (Default)"));
        EditorGUILayout.PropertyField(_colorMouseOver, new GUIContent("Color (Mouse Over)"));

        EditorGUILayout.PropertyField(_scaleOnMouseOver, new GUIContent("Scale (Mouse Over)"));
        if (_scaleOnMouseOver.boolValue)
        {
            EditorGUILayout.Slider(_mouseOverScale, 0, 2, new GUIContent("Factor"));
        }
        EditorGUILayout.PropertyField(_scaleOnClick, new GUIContent("Scale (Click)"));
        if (_scaleOnClick.boolValue)
        {
            EditorGUILayout.Slider(_clickScale, 0, 2, new GUIContent("Factor"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}