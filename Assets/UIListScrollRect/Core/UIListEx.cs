using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIListEx
{
    public static RectOffset GetGridRealPadding<T>(this T gridLayout, int startIndex, int endIndex) where T : UIListLayout, IUIListGrid
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
}
