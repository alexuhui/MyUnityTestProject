using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class UIListVerticalLayout : UIListLayout
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
        return m_IsMirror ? m_ViewportCornerInContentSpace[0].y : -m_ViewportCornerInContentSpace[1].y;
    }

    public override int GetColumnCount()
    {
        return 1;
    }

    public override void ResetPosition()
    {
        this.ResetPositionVerticalEx(m_Content);
    }

    public override void ScrollToItem(int index)
    {
       this.ScrollToItemVerticalEx(index);
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
