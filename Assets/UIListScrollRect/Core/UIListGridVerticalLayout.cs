using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListGridVerticalLayout : UIListVerticalLayout, IUIListGrid
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
        return Mathf.FloorToInt((m_ViewRect.rect.width + m_Spacing.x) / (m_DefaultSize.x + m_Spacing.x));
    }

    public override RectOffset GetRealPadding(int startIndex, int endIndex)
    {
        return this.GetGridRealPadding(startIndex, endIndex);
    }
}
