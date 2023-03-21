using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListGridHorizontalLayout : UIListLayout
{
    public override (bool, bool) InitContent(RectTransform content, RectTransform viewRect,
        List<UIListItemInfo> itemInfos, RectOffset padding,
        Vector2 spacing, int dataCnt,
        int colCnt, Vector2 defSize,
        bool isMirror = false)
    {
        this.InitContentEx(content, viewRect, itemInfos, padding, spacing, dataCnt, colCnt, defSize, isMirror);
        return this.InitContentHorizontalEx(content, isMirror);
    }

    public override float GetStartCorner()
    {
        this.CalViewportCornerEx();
        return -m_ViewportCornerInContentSpace[1].y;
    }

    public override void ResetPosition()
    {
        this.ResetPositionHorizontalEx(m_Content);
    }

    public override int GetColumnCount()
    {
        return Mathf.FloorToInt((m_ViewRect.rect.height + m_Spacing.y) / (m_DefaultSize.y + m_Spacing.y));
    }

    public override void ScrollToItem(int index)
    {
        this.ScrollToItemGridEx(index);
    }

    public override (int, int) GetShowIndex()
    {
        return this.GetShowIndexHorizontalEx();
    }

    public override RectOffset GetRealPadding(int startIndex, int endIndex)
    {
        return this.GetRealPaddingHorizontalEx(startIndex, endIndex);
    }

    

    
}
