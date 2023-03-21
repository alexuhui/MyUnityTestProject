using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UIListLayout
{
    public RectTransform m_Content;
    public RectTransform m_ViewRect;
    public List<UIListItemInfo> m_ItemInfos;
    public RectOffset m_Padding;
    public Vector2 m_Spacing;
    public int m_DataCnt;
    public int m_ColCnt;
    public Vector2 m_DefaultSize;
    public bool m_IsMirror;
    public RectOffset m_RealPadding;
    public Vector3[] m_ViewportCornerInContentSpace = new Vector3[4];

    public abstract (bool, bool) InitContent(RectTransform content, RectTransform viewRect,
        List<UIListItemInfo> itemInfos, RectOffset padding,
        Vector2 spacing, int dataCnt,
        int colCnt, Vector2 defSize,
        bool isMirror = false);

    public abstract float GetStartCorner();
    public abstract int GetColumnCount();
    public abstract void ResetPosition();
    public abstract void ScrollToItem(int index);
    public abstract (int, int) GetShowIndex();
    public abstract void SetRealPadding(int startIndex, int endIndex);
    public abstract Vector2 GetAnchor();
    public virtual void UpdateItemSize(int startIndex, int endIndex) { }
    public abstract void ResetContentPos();
    public abstract void RefreshContentPos(int startIndex, int endIndex);



#if UNITY_EDITOR
    public abstract void Preview();
#endif
}
