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

    public override void SetRealPadding(int startIndex, int endIndex)
    {
        m_RealPadding = this.GetRealPaddingHorizontalEx(startIndex, endIndex);
    }

    public override Vector2 GetAnchor()
    {
        return this.GetAnchorGridEx();
    }
    public override void ResetContentPos()
    {
        this.ResetPosHorizontalEx();
    }

    public override void RefreshContentPos(int startIndex, int endIndex)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            UIListItemInfo itemInfo = m_ItemInfos[i];
            RectTransform rectTransform = itemInfo.render.rectTransform;
            if (i == startIndex)
            {
                rectTransform.anchoredPosition = new Vector2(m_Padding.left, -m_RealPadding.top);
            }
            else
            {
                UIListItemInfo tempItemInfo = m_ItemInfos[i - 1];
                RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                if (i % m_ColCnt == 0)
                    rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + tempItemInfo.size.x + m_Spacing.x, -m_Padding.top);
                else
                    rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x, tempRectTransform.anchoredPosition.y - tempItemInfo.size.y - m_Spacing.y);
            }
        }
        UIListItemInfo lastItemInfo = m_ItemInfos[endIndex];
        RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
        m_Content.sizeDelta = new Vector2(lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_RealPadding.right, m_Content.sizeDelta.y);
    }
}
