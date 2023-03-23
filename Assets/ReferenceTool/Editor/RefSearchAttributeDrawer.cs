using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(RefSearchAttribute))]
public class RefSearchAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - 100, position.height), property);
        if(EditorGUI.LinkButton(new Rect(position.x + position.width - 100, position.y, 100, position.height), "RefSearch"))
        {
            Debug.Log("LinkButton");
        }
    }
}
