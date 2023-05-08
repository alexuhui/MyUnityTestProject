using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIListScrollRect), true)]
public class UIListViewEditor : ScrollRectEditor
{
    SerializedProperty m_ItemPrefab;
    SerializedProperty m_Layout;
    SerializedProperty m_Padding;
    SerializedProperty m_isMirror;
    SerializedProperty m_NotDrag;
    SerializedProperty m_ColCount;
    SerializedProperty m_Spacing;

    UIListScrollRect m_ListView;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_ItemPrefab = serializedObject.FindProperty("ItemPrefab");
        m_Layout = serializedObject.FindProperty("Layout");
        m_Padding = serializedObject.FindProperty("m_Padding");
        m_Spacing = serializedObject.FindProperty("m_Spacing");
        m_isMirror = serializedObject.FindProperty("m_IsMirror");
        m_NotDrag = serializedObject.FindProperty("m_NotDrag");
        m_ColCount = serializedObject.FindProperty("m_ColCount");

        m_ListView = target as UIListScrollRect;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_ItemPrefab);
        EditorGUILayout.PropertyField(m_Layout);
        EditorGUILayout.PropertyField(m_Padding);

        if(m_Layout.enumValueIndex == (int)UIListViewLayout.Horizontal ||
            m_Layout.enumValueIndex == (int)UIListViewLayout.Vertical)
        {
            EditorGUILayout.PropertyField(m_isMirror);
        }

        EditorGUILayout.PropertyField(m_NotDrag);

        EditorGUILayout.PropertyField(m_ColCount);
        EditorGUILayout.PropertyField(m_Spacing);
        EditorGUILayout.Space(15);


        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        if (GUI.changed)
        {
            m_ListView.Preview();
        }
    }
}
