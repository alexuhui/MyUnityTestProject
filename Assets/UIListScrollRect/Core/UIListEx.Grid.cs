using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UIListEx
{
    public static Vector2 GetAnchorGridEx<T>(this T layout, bool isMirror) where T : UIListLayout
    {
        return new Vector2(0, 1);
    }
}
