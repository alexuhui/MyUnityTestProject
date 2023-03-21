using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIListScrollRect : ScrollRect
{
    public UIListItemRender ItemPrefab;

    private UIListLayout m_ListLayout;
    [SerializeField]
    private RectOffset m_Padding;
    [SerializeField]
    private Vector2 m_Spacing = new Vector2(4, 4);
    [SerializeField]
    private int m_ColCount = 2;
    [SerializeField]
    private bool m_IsMirror;
    private int m_SelectedIndex = -1;
    private bool m_IsInit;
    private bool m_IsInvalid;
    private int m_StartIndex;
    private int m_EndIndex;
    private bool m_IsUpdateSize;
    private bool m_IsScrollOnUpdateEnd;
    private int m_ScrollToIndex;

    private RectOffset m_RealPadding;
    private List<ItemDataBase> m_Datas = new List<ItemDataBase>();
    private List<UIListItemInfo> m_ItemInfos = new List<UIListItemInfo>();
    private List<UIListItemRender> m_Renders = new List<UIListItemRender>();


    [SerializeField]
    private UIListViewLayout m_Layout;
    public UIListViewLayout Layout
    {
        get { return m_Layout; }
        set
        {
            m_Layout = value;
        }
    }

    private Vector2 m_DefSize;
    public Vector2 DefSize
    {
        get
        {
            return m_DefSize;
        }
    }

    private RectTransform m_ViewRectTrans;
    public RectTransform ViewRectTrans
    {
        get
        {
            if (m_ViewRectTrans == null)
            {
                m_ViewRectTrans = GetComponent<RectTransform>();
            }
            return m_ViewRectTrans;
        }
    }

    protected override void OnEnable()
    {
        Init();
    }

    protected override void LateUpdate()
    {
        Vector3 scale = this.transform.lossyScale;
        if (scale.x == 0 || scale.y == 0)
            return;

        if (m_IsUpdateSize)
        {
            m_IsUpdateSize = false;
            UpdateSize();
        }
        base.LateUpdate();
    }

    private void Update()
    {
        if (m_IsInvalid && gameObject.activeInHierarchy)
        {
            UpdateView();
            m_IsInvalid = false;
        }
    }

    private void Invalidate()
    {
        m_IsInvalid = true;
    }

    private void InvalidateSize()
    {
        m_IsUpdateSize = true;
    }

    private void Init()
    {
        if (m_IsInit)
            return;
        m_IsInit = true;

        Layout = m_Layout;
        onValueChanged.AddListener(OnScrollRectValueChange);
        ResetHelper();
        InitDefSize();
    }

    private void InitDefSize()
    {
        UIListItemRender temp = CreateItem(false);
        m_DefSize = temp.rectTransform.rect.size;
        if (!Application.isPlaying)
            DestroyImmediate(temp.gameObject);
        else
        {
            temp.gameObject.SetActive(false);
            m_Renders.Add(temp);
        }
    }

    private void ResetHelper()
    {
        switch (m_Layout)
        {
            case UIListViewLayout.Horizontal:
                m_ListLayout = new UIListHorizontalLayout();
                break;
            case UIListViewLayout.Vertical:
                m_ListLayout = new UIListGridVerticalLayout();
                break;
            case UIListViewLayout.GridHorizontal:
                m_ListLayout = new UIListGridHorizontalLayout();
                break;
            case UIListViewLayout.GridVertical:
                m_ListLayout = new UIListGridVerticalLayout();
                break;
            default:
                Debug.LogError($"layout not exit {m_Layout}");
                break;
        }
        (vertical, horizontal) = m_ListLayout.InitContent(content, viewRect, m_ItemInfos, m_Padding, m_Spacing, m_Datas.Count, m_ColCount, DefSize, m_IsMirror);
    }

    private void OnScrollRectValueChange(Vector2 call)
    {
        UpdateView();
    }


    #region Data
    public void AddDatas(List<ItemDataBase> datas)
    {
        m_Datas.AddRange(datas);
        ResetItemInfo();
        Invalidate();
    }

    public void AddData(ItemDataBase data)
    {
        m_Datas.Add(data);
        ResetItemInfo();
        Invalidate();
    }

    public void InsertData(ItemDataBase data, int index)
    {
        m_Datas.Insert(index, data);
        ResetItemInfo();
        if (index <= m_SelectedIndex)
            SetSelect(m_SelectedIndex++);
        Invalidate();
    }

    public void UpdateDataAt(ItemDataBase data, int index)
    {
        if (index >= 0 && index < m_Datas.Count)
        {
            m_Datas[index] = data;
            Invalidate();
        }
    }

    public void RemoveDataAt(int index)
    {
        if (index >= 0 && index < m_Datas.Count)
        {
            m_Datas.RemoveAt(index);
            ResetItemInfo();
            if (index <= m_SelectedIndex)
                SetSelect(m_SelectedIndex--);
            Invalidate();
        }
    }

    public void RemoveData(ItemDataBase data)
    {
        int index = m_Datas.IndexOf(data);
        RemoveDataAt(index);
    }

    public void ClearAll()
    {
        m_Datas.Clear();
        ResetItemInfo();
        m_SelectedIndex = -1;
        Invalidate();
    }
    #endregion


    private void ResetItemInfo()
    {
        int realCount = m_Datas.Count;
        int oldCount = m_ItemInfos.Count;
        if (realCount > oldCount)
        {
            for (int i = oldCount; i < realCount; i++)
            {
                UIListItemInfo itemInfo = new UIListItemInfo();
                itemInfo.size = DefSize;

                m_ItemInfos.Add(itemInfo);
            }
        }
    }

    public void ResetPosition()
    {
        StopMovement();
        m_ListLayout.ResetPosition();
    }

    public void ScrollToItem(int index)
    {
        if (!content)
        {
            return;
        }

        if (m_IsUpdateSize || m_IsInvalid)
        {
            m_IsScrollOnUpdateEnd = true;
            m_ScrollToIndex = index;
            return;
        }

        if (index < 0 || index >= m_Datas.Count)
        {
            return;
        }

        StopMovement();
        m_ListLayout.ScrollToItem(index);
        Invalidate();
    }

    public void SetSelect(int index)
    {
        if (m_SelectedIndex != index)
        {
            int oldIndex = m_SelectedIndex;

            if (index >= 0 && index < m_Datas.Count)
            {
                m_SelectedIndex = index;

                UIListItemRender render = m_ItemInfos[m_SelectedIndex].render;
                if (!m_IsInvalid && render)
                    render.SetSelected(true);
            }
            else if (index < 0)
                m_SelectedIndex = -1;

            if (oldIndex != m_SelectedIndex)
            {
                if (oldIndex >= 0 && oldIndex < m_Datas.Count)
                {
                    UIListItemRender render = m_ItemInfos[oldIndex].render;
                    if (render)
                        render.SetSelected(false);
                }
            }
        }
    }

    private void UpdateView()
    {
        if (!content)
            return;

        (int startIndex, int endIndex) = m_ListLayout.GetShowIndex();

        int maxIndex = Mathf.Max(0, m_Datas.Count - 1);
        if (startIndex <= maxIndex)
        {
            startIndex = Mathf.Min(maxIndex, startIndex);
            endIndex = Mathf.Min(maxIndex, endIndex);
        }
        else
        {
            startIndex = m_StartIndex;
            endIndex = m_EndIndex;
        }

        bool bIsDirty = m_IsInvalid || startIndex != m_StartIndex || endIndex != m_EndIndex;
        if (!bIsDirty)
            return;
        m_StartIndex = startIndex;
        m_EndIndex = endIndex;
        CacheItems();
        RetSetItemsData();
        m_RealPadding = m_ListLayout.GetRealPadding(startIndex, endIndex);
        

        InvalidateSize();
    }


    private UIListItemRender CreateItem(bool bInit = true)
    {
        UIListItemRender render = ItemPrefab.Clone();
        Vector2 v2 = Vector2.zero;
        switch (Layout)
        {
            case UIListViewLayout.Vertical:
                v2 = m_IsMirror ? new Vector2(0, 0) : new Vector2(0, 1);
                break;
            case UIListViewLayout.Horizontal:
                v2 = m_IsMirror ? new Vector2(1, 1) : new Vector2(0, 1);
                break;
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.GridHorizontal:
                v2 = new Vector2(0, 1);
                break;
        }

        render.rectTransform.pivot = render.rectTransform.anchorMin = render.rectTransform.anchorMax = v2;

        render.rectTransform.SetParent(content);
        render.rectTransform.localScale = Vector3.one;
        render.rectTransform.localRotation = Quaternion.identity;
        render.rectTransform.localPosition = Vector3.zero;
        render.OnRectSizeChange = InvalidateSize;

        return render;
    }

    private void OnClickItem(GameObject obj, PointerEventData eventData)
    {
        SetSelect(obj.GetComponent<UIListItemRender>().Index);
    }

    private void UpdateSize()
    {
        if (Layout != UIListViewLayout.GridVertical && Layout != UIListViewLayout.GridHorizontal)
            for (int i = m_StartIndex; i <= m_EndIndex; i++)
            {
                if (i >= m_Datas.Count)
                    continue;

                m_ItemInfos[i].UpdateSize();
            }

        if (m_Datas.Count > 0)
            UpdatePos();
        else
            RestPos();

        if (m_IsScrollOnUpdateEnd)
        {
            m_IsScrollOnUpdateEnd = false;
            ScrollToItem(m_ScrollToIndex);
        }
    }

    private void RestPos()
    {
        switch (Layout)
        {
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.Vertical:
                content.sizeDelta = new Vector2(content.sizeDelta.x, 0);
                break;
            case UIListViewLayout.GridHorizontal:
            case UIListViewLayout.Horizontal:
                content.sizeDelta = new Vector2(0, content.sizeDelta.y);
                break;
        }
    }

    private void UpdatePos()
    {
        switch (Layout)
        {
            case UIListViewLayout.Vertical:
                {
                    for (int i = m_StartIndex; i <= m_EndIndex; i++)
                    {
                        UIListItemInfo itemInfo = m_ItemInfos[i];
                        RectTransform rectTransform = itemInfo.render.rectTransform;
                        if (i == m_StartIndex)
                            rectTransform.anchoredPosition = m_IsMirror ? new Vector2(m_Padding.left, m_RealPadding.top) : new Vector2(m_Padding.left, -m_RealPadding.top);
                        else
                        {
                            UIListItemInfo tempItemInfo = m_ItemInfos[i - 1];
                            RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                            rectTransform.anchoredPosition = m_IsMirror ? new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y + tempItemInfo.size.y + m_Spacing.y) : new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y - tempItemInfo.size.y - m_Spacing.y);
                        }
                    }
                    UIListItemInfo lastItemInfo = m_ItemInfos[m_EndIndex];
                    RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
                    float height = m_IsMirror ? lastRectTransform.anchoredPosition.y + lastItemInfo.size.y + m_RealPadding.bottom : -lastRectTransform.anchoredPosition.y + lastItemInfo.size.y + m_RealPadding.bottom;
                 
                    content.sizeDelta = new Vector2(content.sizeDelta.x, height);
                }
                break;
            case UIListViewLayout.Horizontal:
                {
                    for (int i = m_StartIndex; i <= m_EndIndex; i++)
                    {
                        UIListItemInfo itemInfo = m_ItemInfos[i];
                        RectTransform rectTransform = itemInfo.render.rectTransform;
                        if (i == m_StartIndex)
                            rectTransform.anchoredPosition = m_IsMirror ? new Vector2(-m_RealPadding.left, -m_RealPadding.top) : new Vector2(m_RealPadding.left, -m_RealPadding.top);
                        else
                        {
                            UIListItemInfo tempItemInfo = m_ItemInfos[i - 1];
                            RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                            rectTransform.anchoredPosition = m_IsMirror ? new Vector2(tempRectTransform.anchoredPosition.x - tempItemInfo.size.x - m_Spacing.x, -m_RealPadding.top) : new Vector2(tempRectTransform.anchoredPosition.x + tempItemInfo.size.x + m_Spacing.x, -m_RealPadding.top);
                        }
                    }
                    UIListItemInfo lastItemInfo = m_ItemInfos[m_EndIndex];
                    RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
                    float width = m_IsMirror ? -lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_RealPadding.right : lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_RealPadding.right;
                   
                    content.sizeDelta = new Vector2(width, content.sizeDelta.y);
                }
                break;
            case UIListViewLayout.GridVertical:
                {
                    for (int i = m_StartIndex; i <= m_EndIndex; i++)
                    {
                        UIListItemInfo itemInfo = m_ItemInfos[i];
                        RectTransform rectTransform = itemInfo.render.rectTransform;
                        if (i == m_StartIndex)
                        {
                            rectTransform.anchoredPosition = new Vector2(m_Padding.left, -m_RealPadding.top);
                        }
                        else
                        {
                            int index = i + 1;
                            UIListItemInfo tempItemInfo = m_ItemInfos[i - 1];
                            RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                            if ((i + 1) % m_ColCount == 1)
                                rectTransform.anchoredPosition = new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y - tempItemInfo.size.y - m_Spacing.y);
                            else
                                rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + tempItemInfo.size.x + m_Spacing.x, tempRectTransform.anchoredPosition.y);
                        }
                    }
                    UIListItemInfo lastItemInfo = m_ItemInfos[m_EndIndex];
                    RectTransform lastRectTransform = lastItemInfo.render.rectTransform;

                    float width = m_ColCount * lastItemInfo.size.x + (m_ColCount - 1) * m_Spacing.x + m_RealPadding.left + m_RealPadding.right;//m_IsMirror ? -lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_RealPadding.right : lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_RealPadding.right;
                    
                    content.sizeDelta = new Vector2(content.sizeDelta.x, -lastRectTransform.anchoredPosition.y + lastItemInfo.size.y + m_RealPadding.bottom);
                }
                break;
            case UIListViewLayout.GridHorizontal:
                {
                    for (int i = m_StartIndex; i <= m_EndIndex; i++)
                    {
                        UIListItemInfo itemInfo = m_ItemInfos[i];
                        RectTransform rectTransform = itemInfo.render.rectTransform;
                        if (i == m_StartIndex)
                        {
                            rectTransform.anchoredPosition = new Vector2(m_Padding.left, -m_RealPadding.top);
                        }
                        else
                        {
                            int index = i + 1;
                            UIListItemInfo tempItemInfo = m_ItemInfos[i - 1];
                            RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                            if (i % m_ColCount == 0)
                                rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + DefSize.x + m_Spacing.x, -m_Padding.top);
                            else
                                rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x, tempRectTransform.anchoredPosition.y - DefSize.y - m_Spacing.y);
                        }
                    }
                    UIListItemInfo lastItemInfo = m_ItemInfos[m_EndIndex];
                    RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
                    content.sizeDelta = new Vector2(lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_RealPadding.right, content.sizeDelta.y);
                }
                break;
        }
    }

    private void RetSetItemsData()
    {
        for (int i = m_StartIndex; i <= m_EndIndex; i++)
        {
            if (i >= m_Datas.Count)
                break;

            UIListItemRender render = m_ItemInfos[i].render;
            if (render == null)
            {
                if (m_Renders.Count > 0)
                {
                    int index = m_Renders.Count - 1;
                    render = m_Renders[index];
                    render.gameObject.SetActive(true);
                    m_Renders.RemoveAt(index);
                }
                else
                {
                    render = CreateItem();
                    render.gameObject.SetActive(true);
                }

                render.name = i.ToString();
                //render.m_index = i;
                m_ItemInfos[i].render = render;

                render.SetData(m_Datas[i]);
                render.SetSelected(m_SelectedIndex == i);
            }
            else if (m_IsInvalid || i < this.m_StartIndex || i > this.m_EndIndex)
            {
                render.SetData(m_Datas[i]);
                render.SetSelected(m_SelectedIndex == i);
            }
        }
    }

    private void CacheItems()
    {
        if (m_Datas.Count > 0)
        {
            for (int i = this.m_StartIndex; i < m_StartIndex; i++)
            {
                if (i >= m_ItemInfos.Count)
                    break;

                Cache(m_ItemInfos[i]);
            }

            for (int i = m_EndIndex + 1; i <= this.m_EndIndex; i++)
            {
                if (i >= m_ItemInfos.Count)
                    break;

                Cache(m_ItemInfos[i]);
            }
        }
        else
        {
            for (int i = this.m_StartIndex; i <= this.m_EndIndex; i++)
            {
                if (i >= m_ItemInfos.Count)
                    break;

                Cache(m_ItemInfos[i]);
            }
        }
    }

    private void Cache(UIListItemInfo itemInfo)
    {
        UIListItemRender render = itemInfo.render;
        if (render != null)
        {
            render.gameObject.SetActive(false);
            m_Renders.Add(render);
            itemInfo.render = null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Preview")]
    public void Preview()
    {
        if (Application.isPlaying || !content)
            return;

        Layout = m_Layout;
        Vector2 v2 = Vector2.zero;

        switch (Layout)
        {
            case UIListViewLayout.Vertical:
                v2 = m_IsMirror ? new Vector2(0, 0) : new Vector2(0, 1);
                for (int i = 0; i < content.childCount; i++)
                {
                    RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                    rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = v2;
                    if (i == 0)
                        rectTransform.anchoredPosition = m_IsMirror ? new Vector2(m_Padding.left, m_Padding.bottom) : new Vector2(m_Padding.left, -m_Padding.top);
                    else
                    {
                        RectTransform tempRectTransform = content.GetChild(i - 1).GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = m_IsMirror ? new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y + tempRectTransform.rect.size.y + m_Spacing.y) : new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y - tempRectTransform.rect.size.y - m_Spacing.y);
                    }
                }
                
                break;
            case UIListViewLayout.Horizontal:
                v2 = m_IsMirror ? new Vector2(1, 1) : new Vector2(0, 1);
                for (int i = 0; i < content.childCount; i++)
                {
                    RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                    rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = v2;
                    if (i == 0)
                        rectTransform.anchoredPosition = m_IsMirror ? new Vector2(-m_Padding.right, -m_Padding.top) : new Vector2(m_Padding.left, -m_Padding.top);
                    else
                    {
                        RectTransform tempRectTransform = content.GetChild(i - 1).GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = m_IsMirror ? new Vector2(tempRectTransform.anchoredPosition.x - tempRectTransform.rect.size.x - m_Spacing.y, -m_Padding.top) : new Vector2(tempRectTransform.anchoredPosition.x + tempRectTransform.rect.size.x + m_Spacing.y, -m_Padding.top);
                    }
                }
                
                break;
            case UIListViewLayout.GridVertical:
                v2 = new Vector2(0, 1);
                for (int i = 0; i < content.childCount; i++)
                {
                    RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                    rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = v2;
                    if (i == 0)
                        rectTransform.anchoredPosition = new Vector2(m_Padding.left, -m_Padding.top);
                    else
                    {
                        int index = i + 1;
                        RectTransform tempRectTransform = content.GetChild(i - 1).GetComponent<RectTransform>();
                        if ((i + 1) % m_ColCount == 1)
                            rectTransform.anchoredPosition = new Vector2(m_Padding.left, tempRectTransform.anchoredPosition.y - DefSize.y - m_Spacing.y);
                        else
                            rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + DefSize.x + m_Spacing.x, tempRectTransform.anchoredPosition.y);
                    }
                }
                break;
            case UIListViewLayout.GridHorizontal:
                v2 = new Vector2(0, 1);
                for (int i = 0; i < content.childCount; i++)
                {
                    RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                    rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = v2;
                    if (i == 0)
                        rectTransform.anchoredPosition = new Vector2(m_Padding.left, -m_Padding.top);
                    else
                    {
                        int index = i + 1;
                        RectTransform tempRectTransform = content.GetChild(i - 1).GetComponent<RectTransform>();
                        if (i % m_ColCount == 0)
                            rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + DefSize.x + m_Spacing.x, -m_Padding.top);
                        else
                            rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x, tempRectTransform.anchoredPosition.y - DefSize.y - m_Spacing.y);
                    }
                }
                break;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (Application.isPlaying)
            Invalidate();
    }
#endif
}
