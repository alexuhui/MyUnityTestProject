using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMGUI : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("≤‚ ‘IMGUI", GUILayout.Width(500), GUILayout.Height(50));
        if(GUILayout.Button("≤‚ ‘IMGUI", GUILayout.Width(500), GUILayout.Height(50)))
        {
            Debug.Log("≤‚ ‘IMGUI");
        }
        GUILayout.EndHorizontal();
        GUILayout.HorizontalSlider(0, 100f, 50f, GUILayout.Width(500), GUILayout.Height(50));
    }

    private void Start()
    {
        Debug.Log("vs debug test");
    }
}
