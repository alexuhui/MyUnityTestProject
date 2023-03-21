using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListItemInfo
{
    public UIListItemRender render;
    public Vector2 size;

    public void UpdateSize()
    {
        if (!render)
            return;

        size = render.rectTransform.sizeDelta;
    }
}
