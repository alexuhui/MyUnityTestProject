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

    public virtual (bool, bool) InitContent(RectTransform content, RectTransform viewRect, 
        List<UIListItemInfo> itemInfos, RectOffset padding, 
        Vector2 spacing, int dataCnt, 
        int colCnt, Vector2 defSize,
        bool isMirror = false)
    {
        m_Content = content;
        m_ViewRect = viewRect; 
        m_ItemInfos = itemInfos;
        m_Padding = padding;
        m_Spacing = spacing;
        m_DataCnt = dataCnt;
        m_ColCnt = colCnt;
        m_DefaultSize = defSize;
        m_IsMirror = isMirror;

        return (false, false);
    }

    public abstract void ResetPosition();

    public abstract void ScrollToItem(int index);

    protected Vector3[] m_ViewportCornerInContentSpace = new Vector3[4];
    public float GetStartCorner()
    {
        m_ViewRect.GetWorldCorners(m_ViewportCornerInContentSpace);
        Matrix4x4 matrix4X4 = m_Content.transform.worldToLocalMatrix;
        for (int i = 0; i < 4; ++i)
        {
            m_ViewportCornerInContentSpace[i] = matrix4X4.MultiplyPoint(m_ViewportCornerInContentSpace[i]);
        }
        return Mathf.Round(InnerGetStartCorner());
    }

    protected abstract float InnerGetStartCorner();

    public virtual int GetColumnCount()
    {
        return 2;
    }

    public abstract (int, int) GetShowIndex();

    public abstract RectOffset GetRealPadding(int startIndex, int endIndex);
}
