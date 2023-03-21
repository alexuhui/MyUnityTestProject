using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UIListEx
{
    public static RectOffset GetGridRealPaddingGridEx<T>(this T gridLayout, int startIndex, int endIndex) where T : UIListLayout
    {
        RectOffset padding = new RectOffset(gridLayout.m_Padding.left, gridLayout.m_Padding.right, gridLayout.m_Padding.top, gridLayout.m_Padding.bottom);
        int maxCnt = Mathf.CeilToInt((gridLayout.m_DataCnt * 1f) / gridLayout.m_ColCnt) * gridLayout.m_ColCnt;
        float startPos = padding.top;
        for (int i = 0; i < startIndex; i += gridLayout.m_ColCnt)
        {
            startPos += gridLayout.m_DefaultSize.y;
            if (i < maxCnt)
                startPos += gridLayout.m_Spacing.y;
        }
        padding.top = Mathf.RoundToInt(startPos);

        startPos = padding.bottom;
        for (int i = endIndex + gridLayout.m_ColCnt; i < maxCnt; i += gridLayout.m_ColCnt)
        {
            startPos += gridLayout.m_DefaultSize.y;
            if (i < maxCnt)
                startPos += gridLayout.m_Spacing.y;
        }
        padding.bottom = Mathf.RoundToInt(startPos);

        return padding;
    }

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
}
