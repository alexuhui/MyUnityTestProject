using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogUtils
{
    public static void LogInfo(string content, string tag = "")
    {
        Debug.Log($"{tag}   content");
    }

    public static void LogError(string content, string tag = "")
    {
        Debug.LogError($"{tag}   content");
    }
}
