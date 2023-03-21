using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListLayout_Horizontal : UIListLayout
{
    public override (bool, bool) InitLayout(RectTransform content, RectTransform viewRect,
        List<UIListItemInfo> itemInfos, RectOffset padding,
        Vector2 spacing, List<ItemDataBase> dataList,
        int colCnt, Vector2 defSize,
        bool isMirror = false)
    {
        this.InitContentEx(content, viewRect, itemInfos, padding, spacing, dataList, colCnt, defSize, isMirror);
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

    public override void SetRealPadding(int startIndex, int endIndex)
    {
        m_RealPadding = this.GetRealPaddingHorizontalEx(startIndex, endIndex);
    }

    public override Vector2 GetAnchor()
    {
        return this.GetAnchorHorizontalEx();
    }

    public override void UpdateItemSize(int startIndex, int endIndex)
    {
        this.UpdateSizeEx(startIndex, endIndex);
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
                rectTransform.anchoredPosition = m_IsMirror ? new Vector2(-m_RealPadding.left, -m_RealPadding.top) : new Vector2(m_RealPadding.left, -m_RealPadding.top);
            else
            {
                UIListItemInfo tempItemInfo = m_ItemInfos[i - 1];
                RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                rectTransform.anchoredPosition = m_IsMirror ? new Vector2(tempRectTransform.anchoredPosition.x - tempItemInfo.size.x - m_Spacing.x, -m_RealPadding.top) : new Vector2(tempRectTransform.anchoredPosition.x + tempItemInfo.size.x + m_Spacing.x, -m_RealPadding.top);
            }
        }
        UIListItemInfo lastItemInfo = m_ItemInfos[endIndex];
        RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
        float width = m_IsMirror ? -lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_RealPadding.right : lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_RealPadding.right;

        m_Content.sizeDelta = new Vector2(width, m_Content.sizeDelta.y);
    }

#if UNITY_EDITOR
    public override void Preview()
    {
        for (int i = 0; i < m_Content.childCount; i++)
        {
            RectTransform rectTransform = m_Content.GetChild(i).GetComponent<RectTransform>();
            rectTransform.pivot = 
                rectTransform.anchorMin = 
                rectTransform.anchorMax = m_IsMirror ? new Vector2(1, 1) : new Vector2(0, 1);

            if (i == 0)
            {
                rectTransform.anchoredPosition = m_IsMirror ? new Vector2(-m_Padding.right, -m_Padding.top) : new Vector2(m_Padding.left, -m_Padding.top);
            }
            else
            {
                RectTransform tempRectTransform = m_Content.GetChild(i - 1).GetComponent<RectTransform>();
                rectTransform.anchoredPosition = m_IsMirror ? 
                    new Vector2(tempRectTransform.anchoredPosition.x - tempRectTransform.rect.size.x - m_Spacing.y, -m_Padding.top) : 
                    new Vector2(tempRectTransform.anchoredPosition.x + tempRectTransform.rect.size.x + m_Spacing.y, -m_Padding.top);
            }
        }
    }
#endif
}
