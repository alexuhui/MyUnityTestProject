using System.Collections.Generic;
using UnityEngine;

public class UIListLayout_Vertical : UIListLayout
{
    public override (bool, bool) InitLayout(RectTransform content, RectTransform viewRect,
        List<UIListItemInfo> itemInfos, RectOffset padding,
        Vector2 spacing, List<ItemDataBase> dataList,
        int colCnt, Vector2 defSize,
        bool isMirror = false, bool notDrag = false)
    {
        this.InitContentEx(content, viewRect, itemInfos, padding, spacing, dataList, colCnt, defSize, isMirror, notDrag);
        return this.InitContentVerticalEx(content, isMirror, notDrag);
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

    public override void SetRealPadding(int startIndex, int endIndex)
    {
        m_RealPadding = this.GetRealPaddingVerticalEx(startIndex, endIndex);
    }

    public override Vector2 GetAnchor(bool isMirror)
    {
        return this.GetAnchorVerticalEx(isMirror);
    }

    public override void UpdateItemSize(int startIndex, int endIndex)
    {
        this.UpdateSizeEx(startIndex, endIndex);
    }

    public override void ResetContentPos()
    {
        this.ResetPosVerticalEx();
    }

    public override void RefreshContentPos(int startIndex, int endIndex)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            UIListItemInfo itemInfo = m_ItemInfos[i];
            RectTransform rectTransform = itemInfo.render.rectTransform;
            if (i == startIndex)
                rectTransform.anchoredPosition = m_IsMirror ? new Vector2(m_Padding.left, m_RealPadding.top) : new Vector2(m_Padding.left, -m_RealPadding.top);
            else
            {
                UIListItemInfo tempItemInfo = m_ItemInfos[i - 1];
                RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                rectTransform.anchoredPosition = m_IsMirror ? new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y + tempItemInfo.size.y + m_Spacing.y) : new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y - tempItemInfo.size.y - m_Spacing.y);
            }
        }
        UIListItemInfo lastItemInfo = m_ItemInfos[endIndex];
        RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
        float height = m_IsMirror ? lastRectTransform.anchoredPosition.y + lastItemInfo.size.y + m_RealPadding.bottom : -lastRectTransform.anchoredPosition.y + lastItemInfo.size.y + m_RealPadding.bottom;

        m_Content.sizeDelta = new Vector2(m_Content.sizeDelta.x, height);
    }



#if UNITY_EDITOR
    public override void Preview()
    {
        for (int i = 0; i < m_Content.childCount; i++)
        {
            RectTransform rectTransform = m_Content.GetChild(i).GetComponent<RectTransform>();
            rectTransform.pivot = 
                rectTransform.anchorMin = 
                rectTransform.anchorMax = m_IsMirror ? new Vector2(0, 0) : new Vector2(0, 1);

            if (i == 0)
            {
                rectTransform.anchoredPosition = m_IsMirror ? new Vector2(m_Padding.left, m_Padding.bottom) : new Vector2(m_Padding.left, -m_Padding.top);
            }
            else
            {
                RectTransform tempRectTransform = m_Content.GetChild(i - 1).GetComponent<RectTransform>();
                rectTransform.anchoredPosition = m_IsMirror ? 
                    new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y + tempRectTransform.rect.size.y + m_Spacing.y) : 
                    new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y - tempRectTransform.rect.size.y - m_Spacing.y);
            }
        }
    }
#endif
}
