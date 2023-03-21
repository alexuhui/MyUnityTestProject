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
        base.InitContent(content, viewRect, itemInfos, padding, spacing, dataCnt, colCnt, defSize, isMirror);

        bool vertical = true;
        bool horizontal = false;
        if (isMirror)
        {
            content.anchorMin = new Vector2(content.anchorMin.x, 0);
            content.anchorMax = new Vector2(content.anchorMax.x, 0);
            content.pivot = new Vector2(0.5f, 0);
        }
        else
        {
            content.anchorMin = new Vector2(content.anchorMin.x, 1);
            content.anchorMax = new Vector2(content.anchorMax.x, 1);
            content.pivot = new Vector2(0.5f, 1);
        }
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);

        return (vertical, horizontal);
    }

    public override void ResetPosition()
    {
        m_Content.anchoredPosition = new Vector2(m_Content.anchoredPosition.x, 0);
    }

    public override void ScrollToItem(int index)
    {
        float tempSize = m_Padding.top;
        for (int i = 0; i < index; i++)
        {
            tempSize += m_ItemInfos[i].size.y;
            if (i != m_DataCnt - 1)
                tempSize += m_Spacing.y;
        }

        if (m_Content.rect.height < m_ViewRect.rect.height)
            tempSize = Mathf.Min(0, tempSize);
        else
            tempSize = Mathf.Min(m_Content.rect.height - m_ViewRect.rect.height, tempSize);

        m_Content.anchoredPosition = new Vector2(m_Content.anchoredPosition.x, m_IsMirror ? -tempSize : tempSize);
    }

    protected override float InnerGetStartCorner()
    {
        return m_IsMirror ? m_ViewportCornerInContentSpace[0].y : -m_ViewportCornerInContentSpace[1].y;
    }

    public override (int, int) GetShowIndex()
    {
        int startIndex = 0;
        int endIndex;
        float startPos;
        startPos = m_IsMirror ? m_Padding.bottom : m_Padding.top;
        for (int i = 0; i < m_DataCnt; i++)
        {
            startPos += m_ItemInfos[i].size.y;
            if (startPos >= GetStartCorner())
            {
                startIndex = i;
                break;
            }
            startPos += m_Spacing.y;
        }

        startPos = m_ViewRect.rect.height + m_Spacing.y;
        endIndex = startIndex + Mathf.CeilToInt(startPos / (m_DefaultSize.y + m_Spacing.y));

        return (startIndex, endIndex);
    }

    public override RectOffset GetRealPadding(int startIndex, int endIndex)
    {
        RectOffset padding = new RectOffset(m_Padding.left, m_Padding.right, m_Padding.top, m_Padding.bottom);
        float startPos = m_IsMirror ? padding.bottom : padding.top;
        for (int i = 0; i < startIndex; i++)
        {
            startPos += m_ItemInfos[i].size.y;
            if (i != m_DataCnt - 1)
                startPos += m_Spacing.y;
        }
        int top = Mathf.RoundToInt(startPos);

        startPos = m_IsMirror ? padding.top : padding.bottom;
        for (int i = endIndex + 1; i < m_DataCnt; i++)
        {
            startPos += m_ItemInfos[i].size.y;
            startPos += m_Spacing.y;
        }
        padding.top = top;
        padding.bottom = Mathf.RoundToInt(startPos);

        return padding;
    }
}
