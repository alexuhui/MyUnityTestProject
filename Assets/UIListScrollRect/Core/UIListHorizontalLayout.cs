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
        base.InitContent(content, viewRect, itemInfos, padding, spacing, dataCnt, colCnt, defSize, isMirror);

        bool vertical = false;
        bool horizontal = true;
        if (isMirror)
        {
            content.anchorMin = new Vector2(1, content.anchorMin.y);
            content.anchorMax = new Vector2(1, content.anchorMax.y);
            content.pivot = new Vector2(1, 0.5f);
        }
        else
        {
            content.anchorMin = new Vector2(0, content.anchorMin.y);
            content.anchorMax = new Vector2(0, content.anchorMax.y);
            content.pivot = new Vector2(0, 0.5f);
        }
        content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);

        return (vertical, horizontal);
    }

    public override void ResetPosition()
    {
        m_Content.anchoredPosition = new Vector2(0, m_Content.anchoredPosition.y);
    }

    public override void ScrollToItem(int index)
    {
        float tempSize = m_Padding.left;
        for (int i = 0; i < index; i++)
        {
            tempSize += m_ItemInfos[i].size.x;
            if (i != m_DataCnt - 1)
                tempSize += m_Spacing.x;
        }

        if (m_Content.rect.width < m_ViewRect.rect.width)
            tempSize = Mathf.Min(0, tempSize);
        else
            tempSize = Mathf.Min(m_Content.rect.width - m_ViewRect.rect.width, tempSize);

        m_Content.anchoredPosition = new Vector2(m_IsMirror ? tempSize : -tempSize, m_Content.anchoredPosition.y);
    }

    protected override float InnerGetStartCorner()
    {
        return m_IsMirror ? -m_ViewportCornerInContentSpace[3].x : m_ViewportCornerInContentSpace[0].x;
    }

    public override (int, int) GetShowIndex()
    {
        int startIndex = 0;
        int endIndex;
        float startPos;

        startPos = m_IsMirror ? m_Padding.right : m_Padding.left;
        for (int i = 0; i < m_DataCnt; i++)
        {
            startPos += m_ItemInfos[i].size.x;
            if (startPos >= GetStartCorner())
            {
                startIndex = i;
                break;
            }
            startPos += m_Spacing.x;
        }

        startPos = m_ViewRect.rect.width + m_Spacing.y;
        endIndex = startIndex + Mathf.CeilToInt(startPos / (m_DefaultSize.x + m_Spacing.y));

        return (startIndex, endIndex);
    }

    public override RectOffset GetRealPadding(int startIndex, int endIndex)
    {
        RectOffset padding = new RectOffset(m_Padding.left, m_Padding.right, m_Padding.top, m_Padding.bottom);
        float startPos = m_IsMirror ? padding.right : padding.left;
        for (int i = 0; i < startIndex; i++)
        {
            startPos += m_ItemInfos[i].size.x;
            if (i != m_DataCnt - 1)
                startPos += m_Spacing.x;
        }
        int left = Mathf.RoundToInt(startPos);

        startPos = m_IsMirror ? padding.left : padding.right;
        for (int i = endIndex + 1; i < m_DataCnt; i++)
        {
            startPos += m_ItemInfos[i].size.x;
            if (i != m_DataCnt - 1)
                startPos += m_Spacing.x;
        }
        padding.left = left;
        padding.right = Mathf.RoundToInt(startPos);
        return padding;
    }
}
