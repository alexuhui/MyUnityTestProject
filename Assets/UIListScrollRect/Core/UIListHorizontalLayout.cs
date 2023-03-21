using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListHorizontalLayout : UIListLayout
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
        return m_IsMirror ? -m_ViewportCornerInContentSpace[3].x : m_ViewportCornerInContentSpace[0].x;
    }

    public override int GetColumnCount()
    {
        return 1;
    }

    public override void ResetPosition()
    {
        this.ResetPositionHorizontalEx(m_Content);
    }

    public override void ScrollToItem(int index)
    {
        this.ScrollToItemHorizontalEx(index);
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
