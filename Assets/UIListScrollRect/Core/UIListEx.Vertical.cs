using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UIListEx
{
    public static (bool, bool) InitContentVerticalEx<T>(this T verticalLayout, RectTransform content,
        bool isMirror = false) where T : UIListLayout
    {
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

    public static void ResetPositionVerticalEx<T>(this T layout, RectTransform content) where T : UIListLayout
    {
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
    }

    public static void ScrollToItemVerticalEx<T>(this T layout, int index) where T : UIListLayout
    {
        float tempSize = layout.m_Padding.top;
        for (int i = 0; i < index; i++)
        {
            tempSize += layout.m_ItemInfos[i].size.y;
            if (i != layout.m_DataCnt - 1)
                tempSize += layout.m_Spacing.y;
        }

        if (layout.m_Content.rect.height < layout.m_ViewRect.rect.height)
            tempSize = Mathf.Min(0, tempSize);
        else
            tempSize = Mathf.Min(layout.m_Content.rect.height - layout.m_ViewRect.rect.height, tempSize);

        layout.m_Content.anchoredPosition = new Vector2(layout.m_Content.anchoredPosition.x, layout.m_IsMirror ? -tempSize : tempSize);
    }

    public static (int, int) GetShowIndexVerticalEx<T>(this T layout) where T : UIListLayout
    {
        int startIndex = 0;
        int endIndex;
        float startPos;
        startPos = layout.m_IsMirror ? layout.m_Padding.bottom : layout.m_Padding.top;
        for (int i = 0; i < layout.m_DataCnt; i++)
        {
            startPos += layout.m_ItemInfos[i].size.y;
            if (startPos >= layout.GetStartCorner())
            {
                startIndex = i;
                break;
            }
            startPos += layout.m_Spacing.y;
        }

        startPos = layout.m_ViewRect.rect.height + layout.m_Spacing.y;
        endIndex = startIndex + Mathf.CeilToInt(startPos / (layout.m_DefaultSize.y + layout.m_Spacing.y));

        return (startIndex, endIndex);
    }

    public static RectOffset GetRealPaddingVerticalEx<T>(this T layout, int startIndex, int endIndex) where T : UIListLayout
    {
        RectOffset padding = new RectOffset(layout.m_Padding.left, layout.m_Padding.right, layout.m_Padding.top, layout.m_Padding.bottom);
        float startPos = layout.m_IsMirror ? padding.bottom : padding.top;
        for (int i = 0; i < startIndex; i++)
        {
            startPos += layout.m_ItemInfos[i].size.y;
            if (i != layout.m_DataCnt - 1)
                startPos += layout.m_Spacing.y;
        }
        int top = Mathf.RoundToInt(startPos);

        startPos = layout.m_IsMirror ? padding.top : padding.bottom;
        for (int i = endIndex + 1; i < layout.m_DataCnt; i++)
        {
            startPos += layout.m_ItemInfos[i].size.y;
            startPos += layout.m_Spacing.y;
        }
        padding.top = top;
        padding.bottom = Mathf.RoundToInt(startPos);

        return padding;
    }

    public static Vector2 GetAnchorVerticalEx<T>(this T layout, bool isMirror) where T : UIListLayout
    {
        return isMirror ? new Vector2(0, 0) : new Vector2(0, 1);
    }

    public static void ResetPosVerticalEx<T>(this T layout) where T : UIListLayout
    { 
        var content = layout.m_Content;
        content.sizeDelta = new Vector2(content.sizeDelta.x, 0);
    }
}