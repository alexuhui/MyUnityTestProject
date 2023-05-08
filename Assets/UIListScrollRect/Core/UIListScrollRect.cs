using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIListScrollRect : ScrollRect
{
    public UIListItemRender ItemPrefab;
    public UIListViewLayout Layout;

    [SerializeField]
    private RectOffset m_Padding;
    [SerializeField]
    private Vector2 m_Spacing = new Vector2(4, 4);
    [SerializeField]
    private int m_ColCount = 2;
    [SerializeField]
    private bool m_IsMirror;
    [SerializeField]
    private bool m_NotDrag = false;
    private int m_SelectedIndex = -1;
    private bool m_IsInit;
    private bool m_IsInvalid;
    private int m_StartIndex;
    private int m_EndIndex;
    private bool m_IsUpdateSize;
    private bool m_IsScrollOnUpdateEnd;
    private int m_ScrollToIndex;
    private UIListLayout m_ListLayout;

    private List<ItemDataBase> m_Datas = new List<ItemDataBase>();
    private List<UIListItemInfo> m_ItemInfos = new List<UIListItemInfo>();
    private List<UIListItemRender> m_Renders = new List<UIListItemRender>();
    
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
        if (m_Datas.Count <= 0)
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
        if (m_Datas.Count <= 0)
            return;

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
        if (content == null || viewRect == null)
            return;

        if (m_IsInit)
            return;

        m_IsInit = true;
        onValueChanged.AddListener(OnScrollRectValueChange);
        
        SetLayout();
        InitDefSize();
        InitLayout();
    }

    private void InitDefSize()
    {
        UIListItemRender temp = CreateItem();
        if (temp == null)
            return;

        m_DefSize = temp.rectTransform.rect.size;
        if (!Application.isPlaying)
        {
            DestroyImmediate(temp.gameObject);
        }
        else
        {
            temp.gameObject.SetActive(false);
            m_Renders.Add(temp);
        }
    }

    private void SetLayout()
    {
        switch (Layout)
        {
            case UIListViewLayout.Horizontal:
                m_ListLayout = new UIListLayout_Horizontal();
                break;
            case UIListViewLayout.Vertical:
                m_ListLayout = new UIListLayout_Vertical();
                break;
            case UIListViewLayout.GridHorizontal:
                m_ListLayout = new UIListLayout_GridHorizontal();
                break;
            case UIListViewLayout.GridVertical:
                m_ListLayout = new UIListLayout_GridVertical();
                break;
            default:
                Debug.LogError($"layout not exit {Layout}");
                break;
        }
    }

    private void InitLayout()
    {
        (vertical, horizontal) = m_ListLayout.InitLayout(content, viewRect, m_ItemInfos, m_Padding, m_Spacing, m_Datas, m_ColCount, DefSize, m_IsMirror, m_NotDrag);
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
        SetSelect(index);
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
                    render.SetSelected(UIListItemSelectStatus.Selected);
            }
            else if (index < 0)
                m_SelectedIndex = -1;

            if (oldIndex != m_SelectedIndex)
            {
                if (oldIndex >= 0 && oldIndex < m_Datas.Count)
                {
                    UIListItemRender render = m_ItemInfos[oldIndex].render;
                    if (render)
                        render.SetSelected(UIListItemSelectStatus.Unselected);
                }
            }
        }
    }

    private void UpdateView()
    {
        if (!content)
            return;

        (int newStartIndex, int newEndIndex) = m_ListLayout.GetShowIndex();

        int maxIndex = Mathf.Max(0, m_Datas.Count - 1);
        if (newStartIndex <= maxIndex)
        {
            newStartIndex = Mathf.Min(maxIndex, newStartIndex);
            newEndIndex = Mathf.Min(maxIndex, newEndIndex);
        }
        else
        {
            newStartIndex = m_StartIndex;
            newEndIndex = m_EndIndex;
        }

        bool bIsDirty = m_IsInvalid || newStartIndex != m_StartIndex || newEndIndex != m_EndIndex;
        if (!bIsDirty)
            return;
        
        m_ListLayout.SetRealPadding(newStartIndex, newEndIndex);
        CacheItems(newStartIndex, newEndIndex);
        RetsetItemsData(newStartIndex, newEndIndex);

        m_StartIndex = newStartIndex;
        m_EndIndex = newEndIndex;
        InvalidateSize();
    }


    private UIListItemRender CreateItem()
    {
        if(ItemPrefab == null) return null;

        UIListItemRender render = ItemPrefab.Clone(this);
        render.rectTransform.pivot = 
            render.rectTransform.anchorMin = 
            render.rectTransform.anchorMax = m_ListLayout.GetAnchor(m_IsMirror);

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

    private void RetsetItemsData(int newStartIndex, int newEndIndex)
    {
        for (int i = newStartIndex; i <= newEndIndex; i++)
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

                render.SetData(m_Datas[i], i);
                render.SetSelected(m_SelectedIndex == i ? 
                    UIListItemSelectStatus.Selected : 
                    UIListItemSelectStatus.Unselected);
            }
            else if (m_IsInvalid || i < m_StartIndex || i > m_EndIndex)
            {
                render.SetData(m_Datas[i], i);
                render.SetSelected(m_SelectedIndex == i ?
                    UIListItemSelectStatus.Selected :
                    UIListItemSelectStatus.Unselected);
            }
        }
    }

    private void CacheItems(int newStartIndex, int newEndIndex)
    {
        if (m_Datas.Count > 0)
        {
            for (int i = m_StartIndex; i < newStartIndex; i++)
            {
                if (i >= m_ItemInfos.Count)
                    break;

                Cache(m_ItemInfos[i]);
            }

            for (int i = newEndIndex + 1; i <= m_EndIndex; i++)
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
    private UIListViewLayout? previewLayout;
    [ContextMenu("Preview")]
    public void Preview()
    {
        if (Application.isPlaying || !content)
            return;
        ChangeProp();
        m_ListLayout.Preview();
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (Application.isPlaying)
        {
            ChangeProp();
            Invalidate();
        }
        else
        {
            Preview();
        }
    }

    private void ChangeProp()
    {
        if (previewLayout == null || previewLayout != Layout)
        {
            previewLayout = Layout;
            SetLayout();
        }
        InitLayout();
    }
#endif
}
