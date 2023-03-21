using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListGridVerticalLayout : UIListLayout
{
    public override (bool, bool) InitContent(RectTransform content, RectTransform viewRect,
         List<UIListItemInfo> itemInfos, RectOffset padding,
         Vector2 spacing, int dataCnt,
         int colCnt, Vector2 defSize,
         bool isMirror = false)
    {
        this.InitContentEx(content, viewRect, itemInfos, padding, spacing, dataCnt, colCnt, defSize, isMirror);
        return this.InitContentVerticalEx(content, isMirror);
    }

    public override float GetStartCorner()
    {
        this.CalViewportCornerEx();
        return -m_ViewportCornerInContentSpace[1].y;
    }

    public override void ScrollToItem(int index)
    {
        this.ScrollToItemGridEx(index);
    }

    public override int GetColumnCount()
    {
        return Mathf.FloorToInt((m_ViewRect.rect.width + m_Spacing.x) / (m_DefaultSize.x + m_Spacing.x));
    }

    public override void ResetPosition()
    {
        this.ResetPositionVerticalEx(m_Content);
    }

    public override (int, int) GetShowIndex()
    {
        return this.GetShowIndexVerticalEx();
    }

    public override RectOffset GetRealPadding(int startIndex, int endIndex)
    {
        return this.GetRealPaddingVerticalEx(startIndex, endIndex);
    }
}
