using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UIListEx
{
    public static void InitContentEx<T>(this T layout, RectTransform content, RectTransform viewRect,
        List<UIListItemInfo> itemInfos, RectOffset padding,
        Vector2 spacing, List<ItemDataBase> dataList,
        int colCnt, Vector2 defSize,
        bool isMirror = false) where T : UIListLayout
    {
        layout.m_Content = content;
        layout.m_ViewRect = viewRect;
        layout.m_ItemInfos = itemInfos;
        layout.m_Padding = padding;
        layout.m_Spacing = spacing;
        layout.m_DataList = dataList;
        layout.m_ColCnt = colCnt;
        layout.m_DefaultSize = defSize;
        layout.m_IsMirror = isMirror;
    }

    public static void CalViewportCornerEx<T>(this T layout) where T : UIListLayout
    {
        layout.m_ViewRect.GetWorldCorners(layout.m_ViewportCornerInContentSpace);
        Matrix4x4 matrix4X4 = layout.m_Content.transform.worldToLocalMatrix;
        for (int i = 0; i < 4; ++i)
        {
            layout.m_ViewportCornerInContentSpace[i] = matrix4X4.MultiplyPoint(layout.m_ViewportCornerInContentSpace[i]);
        }
    }

    public static void UpdateSizeEx<T>(this T layout, int startIndex, int endIndex) where T : UIListLayout
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            layout.m_ItemInfos[i].UpdateSize();
        }
    }
}
