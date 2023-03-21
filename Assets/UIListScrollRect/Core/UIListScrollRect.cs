using System.Collections.Generic;
using UnityEngine;
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

    private List<ItemDataBase> m_Datas = new List<ItemDataBase>();
    private List<UIListItemInfo> m_ItemInfos = new List<UIListItemInfo>();
    private List<UIListItemRender> m_Renders = new List<UIListItemRender>();


    [SerializeField]
    private UIListViewLayout m_Layout;
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
            UpdateItemSize();
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
        onValueChanged.AddListener(OnScrollRectValueChange);
        ResetLayout();
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

    private void ResetLayout()
    {
        switch (m_Layout)
        {
            case UIListViewLayout.Horizontal:
                m_ListLayout = new UIListHorizontalLayout();
                break;
            case UIListViewLayout.Vertical:
                m_ListLayout = new UIListVerticalLayout();
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
        m_ListLayout.SetRealPadding(startIndex, endIndex);
        CacheItems();
        RetsetItemsData();
        InvalidateSize();
    }


    private UIListItemRender CreateItem(bool bInit = true)
    {
        UIListItemRender render = ItemPrefab.Clone();
        render.rectTransform.pivot = 
            render.rectTransform.anchorMin = 
            render.rectTransform.anchorMax = m_ListLayout.GetAnchor();

        render.rectTransform.SetParent(content);
        render.rectTransform.localScale = Vector3.one;
        render.rectTransform.localRotation = Quaternion.identity;
        render.rectTransform.localPosition = Vector3.zero;
        render.OnRectSizeChange = InvalidateSize;
        return render;
    }

    private void UpdateItemSize()
    {
        m_ListLayout.UpdateItemSize(m_StartIndex, m_EndIndex);

        if (m_Datas.Count > 0)
            m_ListLayout.RefreshContentPos(m_StartIndex, m_EndIndex);
        else
            m_ListLayout.ResetContentPos();

        if (m_IsScrollOnUpdateEnd)
        {
            m_IsScrollOnUpdateEnd = false;
            ScrollToItem(m_ScrollToIndex);
        }
    }

    private void RetsetItemsData()
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

    private void OnClickItem(GameObject obj, PointerEventData eventData)
    {
        SetSelect(obj.GetComponent<UIListItemRender>().Index);
    }

#if UNITY_EDITOR
    private UIListViewLayout? previewLayout;
    [ContextMenu("Preview")]
    public void Preview()
    {
        if (Application.isPlaying || !content)
            return;
        if (previewLayout == null || previewLayout != m_Layout)
            ResetLayout();

        m_ListLayout.Preview();
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (Application.isPlaying)
            Invalidate();
        else
            Preview();
    }
#endif
}
