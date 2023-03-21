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
    SerializedProperty m_layout;
    SerializedProperty padding;
    SerializedProperty bIsMirror;
    SerializedProperty colCount;
    SerializedProperty bIsAutoColCount;
    SerializedProperty spacing;
    SerializedProperty gridSpacing;
    SerializedProperty bIsNoDrag;
    SerializedProperty bIsAutoCenter;
    SerializedProperty bViewSizeLimit;
    SerializedProperty maxViewSize;
    SerializedProperty parentScrollRect;
    SerializedProperty bIsAutoSize;
    SerializedProperty fViewMinSize;
    SerializedProperty fViewMaxSize;
    SerializedProperty overrideDefSize;
    SerializedProperty arrow01;
    SerializedProperty arrow02;

    UIListScrollRect listView;

    protected override void OnEnable()
    {
        base.OnEnable();

        itemPrefab = serializedObject.FindProperty("itemPrefab");
        m_layout = serializedObject.FindProperty("m_layout");
        padding = serializedObject.FindProperty("padding");
        spacing = serializedObject.FindProperty("spacing");
        gridSpacing = serializedObject.FindProperty("gridSpacing");
        bIsMirror = serializedObject.FindProperty("bIsMirror");
        colCount = serializedObject.FindProperty("colCount");
        bIsAutoColCount = serializedObject.FindProperty("bIsAutoColCount");
        bIsNoDrag = serializedObject.FindProperty("bIsNoDrag");
        bIsAutoCenter = serializedObject.FindProperty("bIsAutoCenter");
        bViewSizeLimit = serializedObject.FindProperty("bViewSizeLimit");
        maxViewSize = serializedObject.FindProperty("maxViewSize");
        parentScrollRect = serializedObject.FindProperty("parentScrollRect");
        bIsAutoSize = serializedObject.FindProperty("m_bIsAutoSize");
        fViewMinSize = serializedObject.FindProperty("m_fViewMinSize");
        fViewMaxSize = serializedObject.FindProperty("m_fViewMaxSize");
        overrideDefSize = serializedObject.FindProperty("overrideDefSize");
        arrow01 = serializedObject.FindProperty("arrow01");
        arrow02 = serializedObject.FindProperty("arrow02");

        listView = target as UIListScrollRect;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(itemPrefab);
        EditorGUILayout.PropertyField(m_layout);

        if (m_layout.enumValueIndex == (int)UIListScrollRect.UIListViewLayout.GridVertical ||
            m_layout.enumValueIndex == (int)UIListScrollRect.UIListViewLayout.GridHorizontal)
        {
            EditorGUILayout.PropertyField(bIsAutoColCount, new GUIContent("Auto Col Count"));
            if (!bIsAutoColCount.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(colCount);
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUILayout.PropertyField(bIsMirror, new GUIContent("Mirror"));
        }
        EditorGUILayout.PropertyField(bIsAutoCenter, new GUIContent("Auto Center"));
        EditorGUILayout.PropertyField(bViewSizeLimit, new GUIContent("ViewSizeLimit"));
        if(bViewSizeLimit.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(maxViewSize, new GUIContent("MaxViewSize"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(bIsNoDrag, new GUIContent("No Drag"));
        if (bIsNoDrag.boolValue)
        {
            EditorGUILayout.PropertyField(parentScrollRect, new GUIContent("Parent Scroll Rect"));
        }
        else
        {
            if (m_layout.enumValueIndex == (int)UIListScrollRect.UIListViewLayout.GridVertical || m_layout.enumValueIndex == (int)UIListScrollRect.UIListViewLayout.Vertical)
            {
                EditorGUILayout.PropertyField(arrow01, new GUIContent("Top Arrow"));
                EditorGUILayout.PropertyField(arrow02, new GUIContent("Bottom Arrow"));
            }
            else
            {
                EditorGUILayout.PropertyField(arrow01, new GUIContent("Left Arrow"));
                EditorGUILayout.PropertyField(arrow02, new GUIContent("Right Arrow"));
            }
        }

        EditorGUILayout.PropertyField(padding);

        if (m_layout.enumValueIndex == (int)UIListScrollRect.UIListViewLayout.GridVertical ||
            m_layout.enumValueIndex == (int)UIListScrollRect.UIListViewLayout.GridHorizontal)
            EditorGUILayout.PropertyField(gridSpacing);
        else
            EditorGUILayout.PropertyField(spacing);

        EditorGUILayout.PropertyField(bIsAutoSize, new GUIContent("Auto Size"));
        if (bIsAutoSize.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(fViewMinSize, new GUIContent("Min"));
            EditorGUILayout.PropertyField(fViewMaxSize, new GUIContent("Max"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(overrideDefSize, new GUIContent("Override Def Size"));

        EditorGUILayout.Space(15);

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        if (GUI.changed)
        {
            listView.Preview();
        }
    }
}
