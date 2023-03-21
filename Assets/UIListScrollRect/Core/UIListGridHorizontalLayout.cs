using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListGridHorizontalLayout : UIListHorizontalLayout, IUIListGrid
{
    public override void ScrollToItem(int index)
    {
        int rowCount = Mathf.FloorToInt(index / m_ColCnt);
        float tempSize = m_ItemInfos.Count > 0 ? m_Padding.top + rowCount * (m_ItemInfos[0].size.y + m_Spacing.y) : 0;

        if (m_Content.rect.height < m_ViewRect.rect.height)
            tempSize = Mathf.Min(0, tempSize);
        else
            tempSize = Mathf.Min(m_Content.rect.height - m_ViewRect.rect.height, tempSize);

        m_Content.anchoredPosition = new Vector2(m_Content.anchoredPosition.x, m_IsMirror ? -tempSize : tempSize);
    }

    protected override float InnerGetStartCorner()
    {
        return -m_ViewportCornerInContentSpace[1].y;
    }

    public override int GetColumnCount()
    {
        return Mathf.FloorToInt((m_ViewRect.rect.height + m_Spacing.y) / (m_DefaultSize.y + m_Spacing.y));
    }

    public override RectOffset GetRealPadding(int startIndex, int endIndex)
    {
        //RectOffset padding = new RectOffset(m_Padding.left, m_Padding.right, m_Padding.top, m_Padding.bottom);
        //int maxCnt = Mathf.CeilToInt((m_DataCnt * 1f) / m_ColCnt) * m_ColCnt;
        //float startPos = padding.top;
        //for (int i = 0; i < startIndex; i += m_ColCnt)
        //{
        //    startPos += m_DefaultSize.y;
        //    if (i < maxCnt)
        //        startPos += m_Spacing.y;
        //}
        //padding.top = Mathf.RoundToInt(startPos);

        //startPos = padding.bottom;
        //for (int i = endIndex + m_ColCnt; i < maxCnt; i += m_ColCnt)
        //{
        //    startPos += m_DefaultSize.y;
        //    if (i < maxCnt)
        //        startPos += m_Spacing.y;
        //}
        //padding.bottom = Mathf.RoundToInt(startPos);

        //return padding;
        return this.GetGridRealPadding(startIndex, endIndex);
    }
}
