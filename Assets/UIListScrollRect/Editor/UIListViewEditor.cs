using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIListScrollRect), true)]
public class UIListViewEditor : ScrollRectEditor
{
    SerializedProperty itemPrefab;
    SerializedProperty layout;
    SerializedProperty padding;
    SerializedProperty bIsMirror;
    SerializedProperty colCount;
    SerializedProperty spacing;

    UIListScrollRect listView;

    protected override void OnEnable()
    {
        base.OnEnable();

        itemPrefab = serializedObject.FindProperty("ItemPrefab");
        layout = serializedObject.FindProperty("m_Layout");
        padding = serializedObject.FindProperty("m_Padding");
        spacing = serializedObject.FindProperty("m_Spacing");
        bIsMirror = serializedObject.FindProperty("m_IsMirror");
        colCount = serializedObject.FindProperty("m_ColCount");

        listView = target as UIListScrollRect;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(itemPrefab);
        EditorGUILayout.PropertyField(layout);
        EditorGUILayout.PropertyField(padding);
        EditorGUILayout.PropertyField(bIsMirror);
        EditorGUILayout.PropertyField(colCount);
        EditorGUILayout.PropertyField(spacing);
        EditorGUILayout.Space(15);


        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        if (GUI.changed)
        {
            listView.Preview();
        }
    }
}
