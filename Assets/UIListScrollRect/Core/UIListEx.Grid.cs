using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UIListEx
{
    public static void ScrollToItemGridEx<T>(this T layout, int index) where T : UIListLayout
    {
        int rowCount = Mathf.FloorToInt(index / layout.m_ColCnt);
        float tempSize = layout.m_ItemInfos.Count > 0 ? layout.m_Padding.top + rowCount * (layout.m_ItemInfos[0].size.y + layout.m_Spacing.y) : 0;

        if (layout.m_Content.rect.height < layout.m_ViewRect.rect.height)
            tempSize = Mathf.Min(0, tempSize);
        else
            tempSize = Mathf.Min(layout.m_Content.rect.height - layout.m_ViewRect.rect.height, tempSize);

        layout.m_Content.anchoredPosition = new Vector2(layout.m_Content.anchoredPosition.x, layout.m_IsMirror ? -tempSize : tempSize);
    }

    public static Vector2 GetAnchorGridEx<T>(this T layout, bool isMirror) where T : UIListLayout
    {
        return new Vector2(0, 1);
    }
}
