using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UIListEx
{
    public static (bool, bool) InitContentHorizontalEx<T>(this T horizontalLayout, RectTransform content,
       bool isMirror = false) where T : UIListLayout
    {
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

    public static void ScrollToItemHorizontalEx<T>(this T layout,int index) where T: UIListLayout
    {
        float tempSize = layout.m_Padding.left;
        for (int i = 0; i < index; i++)
        {
            tempSize += layout.m_ItemInfos[i].size.x;
            if (i != layout.m_DataCnt - 1)
                tempSize += layout.m_Spacing.x;
        }

        if (layout.m_Content.rect.width < layout.m_ViewRect.rect.width)
            tempSize = Mathf.Min(0, tempSize);
        else
            tempSize = Mathf.Min(layout.m_Content.rect.width - layout.m_ViewRect.rect.width, tempSize);

        layout.m_Content.anchoredPosition = new Vector2(layout.m_IsMirror ? tempSize : -tempSize, layout.m_Content.anchoredPosition.y);
    }

    public static void ResetPositionHorizontalEx<T>(this T layout, RectTransform content) where T : UIListLayout
    {
        content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
    }

    public static (int, int) GetShowIndexHorizontalEx<T>(this T layout) where T : UIListLayout
    {
        int startIndex = 0;
        int endIndex;
        float startPos;

        startPos = layout.m_IsMirror ? layout.m_Padding.right : layout.m_Padding.left;
        for (int i = 0; i < layout.m_DataCnt; i++)
        {
            startPos += layout.m_ItemInfos[i].size.x;
            if (startPos >= layout.GetStartCorner())
            {
                startIndex = i;
                break;
            }
            startPos += layout.m_Spacing.x;
        }

        startPos = layout.m_ViewRect.rect.width + layout.m_Spacing.y;
        endIndex = startIndex + Mathf.CeilToInt(startPos / (layout.m_DefaultSize.x + layout.m_Spacing.y));

        return (startIndex, endIndex);
    }

    public static RectOffset GetRealPaddingHorizontalEx<T>(this T layout, int startIndex, int endIndex) where T : UIListLayout
    {
        RectOffset padding = new RectOffset(layout.m_Padding.left, layout.m_Padding.right, layout.m_Padding.top, layout.m_Padding.bottom);
        float startPos = layout.m_IsMirror ? padding.right : padding.left;
        for (int i = 0; i < startIndex; i++)
        {
            startPos += layout.m_ItemInfos[i].size.x;
            if (i != layout.m_DataCnt - 1)
                startPos += layout.m_Spacing.x;
        }
        int left = Mathf.RoundToInt(startPos);

        startPos = layout.m_IsMirror ? padding.left : padding.right;
        for (int i = endIndex + 1; i < layout.m_DataCnt; i++)
        {
            startPos += layout.m_ItemInfos[i].size.x;
            if (i != layout.m_DataCnt - 1)
                startPos += layout.m_Spacing.x;
        }
        padding.left = left;
        padding.right = Mathf.RoundToInt(startPos);
        return padding;
    }

    public static Vector2 GetAnchorHorizontalEx<T>(this T layout) where T : UIListLayout
    {
        return layout.m_IsMirror ? new Vector2(1, 1) : new Vector2(0, 1);
    }

    public static void ResetPosHorizontalEx<T>(this T layout) where T : UIListLayout
    {
        var content = layout.m_Content;
        content.sizeDelta = new Vector2(0, content.sizeDelta.y);
    }
}
