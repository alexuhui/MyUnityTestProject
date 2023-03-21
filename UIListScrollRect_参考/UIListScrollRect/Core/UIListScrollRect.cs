using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIListScrollRect : ScrollRect
{
    public enum UIListViewLayout
    {
        Vertical,
        Horizontal,
        GridVertical,
        GridHorizontal,
    }

    public class ItemInfo
    {
        public UIListItemRender render;
        public Vector2 size;

        public void UpdateSize()
        {
            if (!render)
                return;

            size = render.rectTransform.sizeDelta;
        }
    }

    public UIListItemRender itemPrefab;
    [SerializeField]
    private UIListViewLayout m_layout;
    public UIListViewLayout layout
    {
        get { return m_layout; }
        set
        {
            m_layout = value;

            if (!content)
                return;

            switch (m_layout)
            {
                case UIListViewLayout.GridVertical:
                case UIListViewLayout.Vertical:
                    vertical = !bIsNoDrag ? true : false;
                    horizontal = false;
                    if (bIsMirror)
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
                    break;
                case UIListViewLayout.GridHorizontal:
                case UIListViewLayout.Horizontal:
                    vertical = false;
                    horizontal = !bIsNoDrag ? true : false;
                    if (bIsMirror)
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
                    break;
            }

        }
    }

    public Vector2? _defSize;
    public Vector2 defSize
    {
        get
        {
            if (_defSize == null)
            {
                if (overrideDefSize == default)
                {
                    UIListItemRender temp = CreateItem(false);
                    _defSize = temp.rectTransform.rect.size;
                    if (!Application.isPlaying)
                        DestroyImmediate(temp.gameObject);
                    else
                    {
                        temp.gameObject.SetActive(false);
                        m_renders.Add(temp);
                    }
                }
                else
                {
                    _defSize = overrideDefSize;
                }
            }

            return _defSize.Value;
        }
    }

    public Vector2 overrideDefSize;

    public RectOffset padding;

    public float spacing = 4;
    public Vector2 gridSpacing;
    public int colCount = 1;
    public bool bIsAutoColCount = true;

    public bool bIsMirror;
    public bool bIsNoDrag = false;
    public bool bIsAutoCenter = false;

    public bool bViewSizeLimit = false;
    public float maxViewSize = 60;

    public ScrollRect parentScrollRect;

    public GameObject arrow01;
    public GameObject arrow02;

    private int m_selectedIndex = -1;
    public int selectedIndex
    {
        get { return m_selectedIndex; }
        set
        {
            if (m_selectedIndex != value)
            {
                int oldIndex = m_selectedIndex;

                if (value >= 0 && value < m_datas.Count)
                {
                    m_selectedIndex = value;

                    UIListItemRender render = m_itemInfos[m_selectedIndex].render;
                    if (!m_bIsInvalid && render)
                        render.SetSelected(true);
                }
                else if (value < 0)
                    m_selectedIndex = -1;

                if (oldIndex != m_selectedIndex)
                {
                    if (oldIndex >= 0 && oldIndex < m_datas.Count)
                    {
                        UIListItemRender render = m_itemInfos[oldIndex].render;
                        if (render)
                            render.SetSelected(false);
                    }
                    if (onChange != null)
                        onChange(m_selectedIndex);
                }
            }
        }
    }

    public object selectedData
    {
        get
        {
            if (selectedIndex >= 0 && selectedIndex < m_datas.Count)
                return m_datas[selectedIndex];
            return null;
        }
    }

    public new RectTransform viewRect { get { return base.viewRect; } }

    public UnityAction<int> onChange;
    public UnityAction onUpdateEnd;

    //public UIEventTriggerListener.PointerEventDelegate fnOnBeginDrag;
    //public UIEventTriggerListener.PointerEventDelegate fnOnDrag;
    //public UIEventTriggerListener.PointerEventDelegate fnOnEndDrag;

    private static Vector3[] helpVecs = new Vector3[4];

    private bool m_bIsInit;

    private bool m_bIsInvalid;
    private int m_startIndex;
    private int m_endIndex;
    private bool m_bIsUpdateSize;
    private bool m_bIsScrollOnUpdateEnd;
    private int m_ScrollToIndex;

    [SerializeField]
    private bool m_bIsAutoSize = false;
    [SerializeField]
    private float m_fViewMinSize = 0;
    [SerializeField]
    private float m_fViewMaxSize = 100;

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

    private RectOffset m_realPadding;
    private RectOffset m_autoCenterPadding = new RectOffset();
    private List<object> m_datas = new List<object>();
    public List<object> dataList { get { return m_datas; } }
    private List<ItemInfo> m_itemInfos = new List<ItemInfo>();
    private List<UIListItemRender> m_renders = new List<UIListItemRender>();

    //private UITweenPosition mPosTw;


    public int startIndex { get { return m_startIndex; } }
    public int endIndex { get { return m_endIndex; } }

    public void AddDatas(List<object> datas)
    {
        m_datas.AddRange(datas);
        ResetItemInfo();
        Invalidate();
    }

    public void AddData(object data)
    {
        m_datas.Add(data);
        ResetItemInfo();
        Invalidate();
    }

    public void InsertData(object data, int index)
    {
        m_datas.Insert(index, data);
        ResetItemInfo();
        if (index <= selectedIndex)
            selectedIndex++;
        Invalidate();
    }

    public void UpdateDataAt(object data, int index)
    {
        if (index >= 0 && index < m_datas.Count)
        {
            m_datas[index] = data;
            Invalidate();
        }
    }

    public void RemoveDataAt(int index)
    {
        if (index >= 0 && index < m_datas.Count)
        {
            m_datas.RemoveAt(index);
            ResetItemInfo();
            if (index <= selectedIndex)
                selectedIndex--;
            Invalidate();
        }
    }

    public void RemoveData(object data)
    {
        int index = m_datas.IndexOf(data);
        RemoveDataAt(index);
    }

    public void ClearAll()
    {
        m_datas.Clear();
        ResetItemInfo();
        m_selectedIndex = -1;
        Invalidate();
    }

    private void ResetItemInfo()
    {
        int realCount = m_datas.Count;
        int oldCount = m_itemInfos.Count;
        if (realCount > oldCount)
        {
            for (int i = oldCount; i < realCount; i++)
            {
                ItemInfo itemInfo = new ItemInfo();
                itemInfo.size = defSize;

                m_itemInfos.Add(itemInfo);
            }
        }
    }

    public void ResetPosition()
    {
        StopMovement();
        switch (layout)
        {
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.Vertical:
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
                break;
            case UIListViewLayout.GridHorizontal:
            case UIListViewLayout.Horizontal:
                content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
                break;
        }
    }

    public void ScrollToItem(int index)
    {
        if (!content)
            return;

        if (m_bIsUpdateSize || m_bIsInvalid)
        {
            m_bIsScrollOnUpdateEnd = true;
            m_ScrollToIndex = index;
            return;
        }

        StopMovement();
        switch (layout)
        {
            case UIListViewLayout.Vertical:
                if (index >= 0 && index < m_datas.Count)
                {
                    float tempSize = padding.top;
                    for (int i = 0; i < index; i++)
                    {
                        tempSize += m_itemInfos[i].size.y;
                        if (i != m_datas.Count - 1)
                            tempSize += spacing;
                    }

                    if (content.rect.height < viewRect.rect.height)
                        tempSize = Mathf.Min(0, tempSize);
                    else
                        tempSize = Mathf.Min(content.rect.height - viewRect.rect.height, tempSize);

                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, bIsMirror ? -tempSize : tempSize);
                }
                break;
            case UIListViewLayout.Horizontal:
                if (index >= 0 && index < m_datas.Count)
                {
                    float tempSize = padding.left;
                    for (int i = 0; i < index; i++)
                    {
                        tempSize += m_itemInfos[i].size.x;
                        if (i != m_datas.Count - 1)
                            tempSize += spacing;
                    }

                    if (content.rect.width < viewRect.rect.width)
                        tempSize = Mathf.Min(0, tempSize);
                    else
                        tempSize = Mathf.Min(content.rect.width - viewRect.rect.width, tempSize);

                    content.anchoredPosition = new Vector2(bIsMirror ? tempSize : -tempSize, content.anchoredPosition.y);
                }
                break;
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.GridHorizontal:
                if (index >= 0 && index < m_datas.Count)
                {
                    int rowCount = Mathf.FloorToInt(index / colCount);
                    float tempSize = padding.top + rowCount * (defSize.y + gridSpacing.y);

                    if (content.rect.height < viewRect.rect.height)
                        tempSize = Mathf.Min(0, tempSize);
                    else
                        tempSize = Mathf.Min(content.rect.height - viewRect.rect.height, tempSize);

                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, bIsMirror ? -tempSize : tempSize);
                }
                break;
        }
        Invalidate();
    }


    /// <summary>
    /// 滚动到指定单元
    /// </summary>
    /// <param name="index">单元在数据中的索引</param>
    /// <param name="moveTime">滚动延迟s</param>
    public void ScrollToCellByTween(int index, float moveTime = 1)
    {
        if (!content)
            return;
        //if (m_bIsUpdateSize || m_bIsInvalid)
        //{
        //    m_bIsScrollOnUpdateEnd = true;
        //    m_ScrollToIndex = index;
        //    return;
        //}
        //if (mPosTw == null)
        //    mPosTw = content.gameObject.AddComponent<UITweenPosition>();
        StopMovement();
        Vector3 toV3 = Vector3.zero;
        switch (layout)
        {
            case UIListViewLayout.Vertical:
                if (index >= 0 && index < m_datas.Count)
                {
                    float tempSize = padding.top;
                    for (int i = 0; i < index; i++)
                    {
                        tempSize += m_itemInfos[i].size.y;
                        if (i != m_datas.Count - 1)
                            tempSize += spacing;
                    }

                    if (content.rect.height < viewRect.rect.height)
                        tempSize = Mathf.Min(0, tempSize);
                    else
                        tempSize = Mathf.Min(content.rect.height - viewRect.rect.height, tempSize);

                    toV3 = new Vector3(content.anchoredPosition.x, bIsMirror ? -tempSize : tempSize, 0);
                }
                break;
            case UIListViewLayout.Horizontal:
                if (index >= 0 && index < m_datas.Count)
                {
                    float tempSize = padding.left;
                    for (int i = 0; i < index; i++)
                    {
                        tempSize += m_itemInfos[i].size.x;
                        if (i != m_datas.Count - 1)
                            tempSize += spacing;
                    }

                    if (content.rect.width < viewRect.rect.width)
                        tempSize = Mathf.Min(0, tempSize);
                    else
                        tempSize = Mathf.Min(content.rect.width - viewRect.rect.width, tempSize);

                    toV3 = new Vector3(bIsMirror ? tempSize : -tempSize, content.anchoredPosition.y, 0);
                }
                break;
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.GridHorizontal:
                if (index >= 0 && index < m_datas.Count)
                {
                    int rowCount = Mathf.FloorToInt(index / colCount);
                    float tempSize = padding.top + rowCount * (defSize.y + gridSpacing.y);

                    if (content.rect.height < viewRect.rect.height)
                        tempSize = Mathf.Min(0, tempSize);
                    else
                        tempSize = Mathf.Min(content.rect.height - viewRect.rect.height, tempSize);

                    toV3 = new Vector3(content.anchoredPosition.x, bIsMirror ? -tempSize : tempSize, 0);
                }
                break;
        }
        //mPosTw.duration = moveTime;
        //mPosTw.from = new Vector3(content.anchoredPosition.x, content.anchoredPosition.y, 0);
        //mPosTw.to = toV3;
        //mPosTw.ResetToBeginning();
        //mPosTw.PlayForward();
    }

    public float GetDistanceFromBottom()
    {
        float value = 0;
        switch (layout)
        {
            case UIListViewLayout.Vertical:
                float y = bIsMirror ? -content.anchoredPosition.y : content.anchoredPosition.y;
                value = content.rect.height - y - viewRect.rect.height;
                break;
            case UIListViewLayout.Horizontal:
                float x = bIsMirror ? content.anchoredPosition.x : -content.anchoredPosition.x;
                value = content.rect.width - x - viewRect.rect.width;
                break;
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.GridHorizontal:
                value = content.rect.height - content.anchoredPosition.y - viewRect.rect.height;
                break;
        }
        return value;
    }

    public UIListItemRender GetItemRenderByIndex(int index)
    {
        if (index >= 0 && index < m_datas.Count)
            return m_itemInfos[index].render;

        return null;
    }

    /// <summary>
    /// 屏幕点转到item
    /// </summary>
    public UIListItemRender ScreenPosToItemRender(float x, float y)
    {
        UIListItemRender tempItem = null;
        Vector2 screenPos = new Vector2(x, y);
        for (int i = 0; i < m_itemInfos.Count; i++)
        {
            tempItem = m_itemInfos[i].render;
            if (tempItem!=null)
            {
                //bool isInRect = RectTransformUtility.RectangleContainsScreenPoint(tempItem.rectTransform, screenPos, UITools.uiCamera);
                //if (isInRect)
                //{
                //    return tempItem;
                //}
            }
        }
        return null;
    }

    private void UpdateNow()
    {
        if (m_bIsInvalid && gameObject.activeInHierarchy)
        {
            UpdateView();
            m_bIsInvalid = false;
        }
    }

    protected override void LateUpdate()
    {
        Vector3 scale = this.transform.lossyScale;
        if (scale.x == 0 || scale.y == 0)
            return;

        //base.LateUpdate();

        if (m_bIsUpdateSize)
        {
            m_bIsUpdateSize = false;
            UpdateSize();
        }
        base.LateUpdate();
    }

    private void Update()
    {
        UpdateNow();
    }

    public void Invalidate()
    {
        m_bIsInvalid = true;
    }

    public void InvalidateSize()
    {
        m_bIsUpdateSize = true;
    }

    private void Init()
    {
        if (m_bIsInit)
            return;
        m_bIsInit = true;

        layout = m_layout;
        onValueChanged.AddListener(OnScrollRectValueChange);

        if (!content)
            Debug.LogErrorFormat("请移除多余ScrollRect组件，并对UIListView的Content和Viewport赋值。\n{0}", transform.name);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (bIsNoDrag)
        {
            if (parentScrollRect != null)
                parentScrollRect.OnBeginDrag(eventData);
            return;
        }

        base.OnBeginDrag(eventData);

        //if (fnOnBeginDrag != null)
        //    fnOnBeginDrag(gameObject, eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (bIsNoDrag)
        {
            if (parentScrollRect != null)
                parentScrollRect.OnDrag(eventData);
            return;
        }

        base.OnDrag(eventData);

        //if (fnOnDrag != null)
        //    fnOnDrag(gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (bIsNoDrag)
        {
            if (parentScrollRect != null)
                parentScrollRect.OnEndDrag(eventData);
            return;
        }

        base.OnEndDrag(eventData);

        //if (fnOnEndDrag != null)
        //    fnOnEndDrag(gameObject, eventData);
    }

    private void OnScrollRectValueChange(Vector2 call)
    {
        UpdateView();

        switch (layout)
        {
            case UIListViewLayout.Vertical:
            case UIListViewLayout.GridVertical:
                {
                    bool bIsShowArrow = content.rect.height > viewRect.rect.height;
                    if (arrow01)
                        arrow01.SetActive(bIsShowArrow && verticalNormalizedPosition < 1);
                    if (arrow02)
                        arrow02.SetActive(bIsShowArrow && verticalNormalizedPosition > 0);
                }
                break;
            case UIListViewLayout.Horizontal:
            case UIListViewLayout.GridHorizontal:
                {
                    bool bIsShowArrow = content.rect.width > viewRect.rect.width;
                    if (arrow01)
                        arrow01.SetActive(bIsShowArrow && horizontalNormalizedPosition > 0);
                    if (arrow02)
                        arrow02.SetActive(bIsShowArrow && horizontalNormalizedPosition < 1);
                }
                break;
        }
    }

    private void UpdateView()
    {
        Init();

        if (!content)
            return;

        viewRect.GetWorldCorners(helpVecs);
        Vector3[] corners = helpVecs;

        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = this.content.transform.InverseTransformPoint(v);
            corners[i] = v;
        }

        float min = 0;
        //float max = 0;
        switch (layout)
        {
            case UIListViewLayout.Vertical:
                min = bIsMirror ? corners[0].y : -corners[1].y;
                //max = bIsMirror ? corners[1].y : -corners[0].y;
                break;
            case UIListViewLayout.Horizontal:
                min = bIsMirror ? -corners[3].x : corners[0].x;
                //max = bIsMirror ? -corners[0].x : corners[3].x;
                break;
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.GridHorizontal:
                min = -corners[1].y;
                //max = -corners[0].y;
                break;
        }

        min = Mathf.Round(min);

        if (bIsAutoColCount)
        {
            if (layout == UIListViewLayout.GridVertical)
                colCount = Mathf.FloorToInt((viewRect.rect.width + gridSpacing.x) / (defSize.x + gridSpacing.x));
            else if (layout == UIListViewLayout.GridHorizontal)
                colCount = Mathf.FloorToInt((viewRect.rect.height + gridSpacing.y) / (defSize.y + gridSpacing.y));
        }

        RectOffset tempPadding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom);

        int? tempValue = null;
        float tempSize;
        switch (layout)
        {
            case UIListViewLayout.Vertical:
                tempSize = bIsMirror ? tempPadding.bottom : tempPadding.top;
                for (int i = 0; i < m_datas.Count; i++)
                {
                    tempSize += m_itemInfos[i].size.y;
                    if (tempValue == null && tempSize >= min)
                    {
                        tempValue = i;
                        break;
                    }
                    tempSize += spacing;
                }
                break;
            case UIListViewLayout.Horizontal:
                tempSize = bIsMirror ? tempPadding.right : tempPadding.left;
                for (int i = 0; i < m_datas.Count; i++)
                {
                    tempSize += m_itemInfos[i].size.x;
                    if (tempValue == null && tempSize >= min)
                    {
                        tempValue = i;
                        break;
                    }
                    tempSize += spacing;
                }
                break;
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.GridHorizontal:
                tempSize = tempPadding.top;
                if (min > tempSize)
                {
                    tempSize = min - tempSize;
                    tempValue = Mathf.FloorToInt(tempSize / (defSize.y + gridSpacing.y)) * colCount;
                }
                else
                    tempValue = 0;
                break;
            default:
                tempSize = 0;
                break;
        }

        int tempStartIndex = tempValue.GetValueOrDefault();
        int tempEndIndex;

        //float viewSize = max - min;
        switch (layout)
        {
            case UIListViewLayout.Vertical:
                tempSize = viewRect.rect.height + spacing;
                tempEndIndex = tempStartIndex + Mathf.CeilToInt(tempSize / (defSize.y + spacing));
                break;
            case UIListViewLayout.Horizontal:
                tempSize = viewRect.rect.width + spacing;
                tempEndIndex = tempStartIndex + Mathf.CeilToInt(tempSize / (defSize.x + spacing));
                break;

            case UIListViewLayout.GridVertical:
            case UIListViewLayout.GridHorizontal:
                tempSize = viewRect.rect.height + gridSpacing.y;
                tempEndIndex = tempStartIndex + Mathf.CeilToInt(tempSize / (defSize.y + gridSpacing.y) + 1) * colCount - 1;
                break;
            default:
                tempEndIndex = tempStartIndex;
                break;
        }

        int maxIndex = Mathf.Max(0, m_datas.Count - 1);
        if (tempStartIndex <= maxIndex)
        {
            tempStartIndex = Mathf.Min(maxIndex, tempStartIndex);
            tempEndIndex = Mathf.Min(maxIndex, tempEndIndex);
        }
        else
        {
            tempStartIndex = m_startIndex;
            tempEndIndex = m_endIndex;
        }

        bool bIsDirty = m_bIsInvalid || tempStartIndex != m_startIndex || tempEndIndex != m_endIndex;

        if (!bIsDirty)
            return;

        switch (layout)
        {
            case UIListViewLayout.Vertical:
                tempSize = bIsMirror ? tempPadding.bottom : tempPadding.top;
                for (int i = 0; i < tempStartIndex; i++)
                {
                    tempSize += m_itemInfos[i].size.y;
                    if (i != m_datas.Count - 1)
                        tempSize += spacing;
                }
                int top = Mathf.RoundToInt(tempSize);

                tempSize = bIsMirror ? tempPadding.top : tempPadding.bottom;
                for (int i = tempEndIndex + 1; i < m_datas.Count; i++)
                {
                    tempSize += m_itemInfos[i].size.y;
                    //if (i != m_datas.Count - 1)
                        tempSize += spacing;
                }
                tempPadding.top = top;
                tempPadding.bottom = Mathf.RoundToInt(tempSize);
                break;
            case UIListViewLayout.Horizontal:
                tempSize = bIsMirror ? tempPadding.right : tempPadding.left;
                for (int i = 0; i < tempStartIndex; i++)
                {
                    tempSize += m_itemInfos[i].size.x;
                    if (i != m_datas.Count - 1)
                        tempSize += spacing;
                }
                int left = Mathf.RoundToInt(tempSize);

                tempSize = bIsMirror ? tempPadding.left : tempPadding.right;
                for (int i = tempEndIndex + 1; i < m_datas.Count; i++)
                {
                    tempSize += m_itemInfos[i].size.x;
                    if (i != m_datas.Count - 1)
                        tempSize += spacing;
                }
                tempPadding.left = left;
                tempPadding.right = Mathf.RoundToInt(tempSize);
                break;
            case UIListViewLayout.GridVertical:
            case UIListViewLayout.GridHorizontal:
                int maxCnt = Mathf.CeilToInt((m_datas.Count * 1f) / colCount) * colCount;

                tempSize = tempPadding.top;
                for (int i = 0; i < tempStartIndex; i += colCount)
                {
                    tempSize += defSize.y;
                    if (i < maxCnt)
                        tempSize += gridSpacing.y;
                }
                tempPadding.top = Mathf.RoundToInt(tempSize);

                tempSize = tempPadding.bottom;
                for (int i = tempEndIndex + colCount; i < maxCnt; i += colCount)
                {
                    tempSize += defSize.y;
                    if (i < maxCnt)
                        tempSize += gridSpacing.y;
                }
                tempPadding.bottom = Mathf.RoundToInt(tempSize);
                break;
        }

        m_realPadding = tempPadding;

        if (m_datas.Count > 0)
        {
            for (int i = m_startIndex; i < tempStartIndex; i++)
            {
                if (i >= m_itemInfos.Count)
                    break;

                Cache(m_itemInfos[i]);
            }

            for (int i = tempEndIndex + 1; i <= m_endIndex; i++)
            {
                if (i >= m_itemInfos.Count)
                    break;

                Cache(m_itemInfos[i]);
            }
        }
        else
        {
            for (int i = m_startIndex; i <= m_endIndex; i++)
            {
                if (i >= m_itemInfos.Count)
                    break;

                Cache(m_itemInfos[i]);
            }
        }

        for (int i = tempStartIndex; i <= tempEndIndex; i++)
        {
            if (i >= m_datas.Count)
                break;

            UIListItemRender render = m_itemInfos[i].render;
            if (render == null)
            {
                if (m_renders.Count > 0)
                {
                    int index = m_renders.Count - 1;
                    render = m_renders[index];
                    bool bNeedInit = !render.bInit;
                    render.TryInit();
                    if (bNeedInit && render.bInit) ;
                        //UIEventTriggerListener.Get(render.gameObject).fnOnPointerClick = OnClickItem + UIEventTriggerListener.Get(render.gameObject).fnOnPointerClick;
                    render.gameObject.SetActive(true);
                    m_renders.RemoveAt(index);
                }
                else
                {
                    render = CreateItem();
                    render.gameObject.SetActive(true);
                }

                render.name = i.ToString();
                render.m_index = i;
                m_itemInfos[i].render = render;

                render.SetData(m_datas[i]);
                render.SetSelected(selectedIndex == i);
            }
            else if (m_bIsInvalid || i < m_startIndex || i > m_endIndex)
            {
                render.SetData(m_datas[i]);
                render.SetSelected(selectedIndex == i);
            }
        }

        m_startIndex = tempStartIndex;
        m_endIndex = tempEndIndex;

        InvalidateSize();
    }


    private void SetViewSize()
    {
        if (!m_bIsAutoSize && !bViewSizeLimit) return;

        var oriSize = ViewRectTrans.sizeDelta;
        var contenSize = content.sizeDelta;
        if (layout == UIListViewLayout.Horizontal || layout == UIListViewLayout.GridHorizontal)
        {
            float size = oriSize.x;
            if (m_bIsAutoSize)
                size = Mathf.Min(Mathf.Max(m_fViewMinSize, contenSize.x), m_fViewMaxSize);
            else if (bViewSizeLimit)
                size = Mathf.Min(maxViewSize, contenSize.x);

            ViewRectTrans.sizeDelta = new Vector2(size, oriSize.y);
        }
        else if (layout == UIListViewLayout.Vertical || layout == UIListViewLayout.GridVertical)
        {
            float size = oriSize.y;
            if (m_bIsAutoSize)
                size = Mathf.Min(Mathf.Max(m_fViewMinSize, contenSize.y), m_fViewMaxSize);
            else
                size = Mathf.Min(maxViewSize, contenSize.y);

            ViewRectTrans.sizeDelta = new Vector2(oriSize.x, size);
        }
    }


    private UIListItemRender CreateItem(bool bInit = true)
    {
        UIListItemRender render = itemPrefab.Clone(bInit);
        Vector2 v2 = Vector2.zero;
        switch (layout)
        {
            case UIListViewLayout.Vertical:
                v2 = bIsMirror ? new Vector2(0, 0) : new Vector2(0, 1);
                break;
            case UIListViewLayout.Horizontal:
                v2 = bIsMirror ? new Vector2(1, 1) : new Vector2(0, 1);
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
        render.onRectSizeChange = InvalidateSize;

        // 由于可能有点击后取消选中的需求，如果OnClickItem在最后调用会导致Lua添加的点击设置了取消选中后又被ListView的点击设置回去
        // 所以优先执行ListView的点击事件以处理这种情况
        //if (bInit)
        //    UIEventTriggerListener.Get(render.gameObject).fnOnPointerClick = OnClickItem + UIEventTriggerListener.Get(render.gameObject).fnOnPointerClick;
        return render;
    }

    private void OnClickItem(GameObject obj, PointerEventData eventData)
    {
        this.selectedIndex = obj.GetComponent<UIListItemRender>().index;
    }

    private void UpdateSize()
    {
        if (layout != UIListViewLayout.GridVertical && layout != UIListViewLayout.GridHorizontal)
            for (int i = m_startIndex; i <= m_endIndex; i++)
            {
                if (i >= m_datas.Count)
                    continue;

                m_itemInfos[i].UpdateSize();
            }

        if (m_datas.Count > 0)
            UpdatePos();
        else
            RestPos();

        if (m_bIsScrollOnUpdateEnd)
        {
            m_bIsScrollOnUpdateEnd = false;
            ScrollToItem(m_ScrollToIndex);
        }

        //if (m_bIsAutoSize)
        //{
        SetViewSize();
        //}

        if (onUpdateEnd != null)
            onUpdateEnd();
    }

    private void RestPos()
    {
        switch (layout)
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
        switch (layout)
        {
            case UIListViewLayout.Vertical:
                {
                    for (int i = m_startIndex; i <= m_endIndex; i++)
                    {
                        ItemInfo itemInfo = m_itemInfos[i];
                        RectTransform rectTransform = itemInfo.render.rectTransform;
                        if (i == m_startIndex)
                            rectTransform.anchoredPosition = bIsMirror ? new Vector2(padding.left, m_realPadding.top) : new Vector2(padding.left, -m_realPadding.top);
                        else
                        {
                            ItemInfo tempItemInfo = m_itemInfos[i - 1];
                            RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                            rectTransform.anchoredPosition = bIsMirror ? new Vector2(padding.left, tempRectTransform.anchoredPosition.y + tempItemInfo.size.y + spacing) : new Vector2(padding.left, tempRectTransform.anchoredPosition.y - tempItemInfo.size.y - spacing);
                        }
                    }
                    ItemInfo lastItemInfo = m_itemInfos[m_endIndex];
                    RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
                    float height = bIsMirror ? lastRectTransform.anchoredPosition.y + lastItemInfo.size.y + m_realPadding.bottom : -lastRectTransform.anchoredPosition.y + lastItemInfo.size.y + m_realPadding.bottom;
                    if (bIsAutoCenter)
                    {
                        float offset = height > viewRect.rect.height ? 0 : (viewRect.rect.height - height) * 0.5f;
                        if (offset > 0)
                        {
                            Vector2 offsetPos = new Vector2(0, bIsMirror ? offset : -offset);
                            for (int i = m_startIndex; i <= m_endIndex; i++)
                            {
                                ItemInfo itemInfo = m_itemInfos[i];
                                RectTransform rectTransform = itemInfo.render.rectTransform;
                                rectTransform.anchoredPosition += offsetPos;
                            }
                            height = viewRect.rect.height;
                            m_autoCenterPadding.top = m_autoCenterPadding.bottom = Mathf.RoundToInt(offset);
                            m_autoCenterPadding.left = m_autoCenterPadding.right = 0;
                        }
                    }
                    content.sizeDelta = new Vector2(content.sizeDelta.x, height);
                }
                break;
            case UIListViewLayout.Horizontal:
                {
                    for (int i = m_startIndex; i <= m_endIndex; i++)
                    {
                        ItemInfo itemInfo = m_itemInfos[i];
                        RectTransform rectTransform = itemInfo.render.rectTransform;
                        if (i == m_startIndex)
                            rectTransform.anchoredPosition = bIsMirror ? new Vector2(-m_realPadding.left, -m_realPadding.top) : new Vector2(m_realPadding.left, -m_realPadding.top);
                        else
                        {
                            ItemInfo tempItemInfo = m_itemInfos[i - 1];
                            RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                            rectTransform.anchoredPosition = bIsMirror ? new Vector2(tempRectTransform.anchoredPosition.x - tempItemInfo.size.x - spacing, -m_realPadding.top) : new Vector2(tempRectTransform.anchoredPosition.x + tempItemInfo.size.x + spacing, -m_realPadding.top);
                        }
                    }
                    ItemInfo lastItemInfo = m_itemInfos[m_endIndex];
                    RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
                    float width = bIsMirror ? -lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_realPadding.right : lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_realPadding.right;
                    if (bIsAutoCenter)
                    {
                        float offset = width > viewRect.rect.width ? 0 : (viewRect.rect.width - width) * 0.5f;
                        if (offset > 0)
                        {
                            Vector2 offsetPos = new Vector2(bIsMirror ? -offset : offset, 0);
                            for (int i = m_startIndex; i <= m_endIndex; i++)
                            {
                                ItemInfo itemInfo = m_itemInfos[i];
                                RectTransform rectTransform = itemInfo.render.rectTransform;
                                rectTransform.anchoredPosition += offsetPos;
                            }
                            width = viewRect.rect.width;
                            m_autoCenterPadding.left = m_autoCenterPadding.right = Mathf.RoundToInt(offset);
                            m_autoCenterPadding.top = m_autoCenterPadding.bottom = 0;
                        }
                    }
                    content.sizeDelta = new Vector2(width, content.sizeDelta.y);
                }
                break;
            case UIListViewLayout.GridVertical:
                {
                    for (int i = m_startIndex; i <= m_endIndex; i++)
                    {
                        ItemInfo itemInfo = m_itemInfos[i];
                        RectTransform rectTransform = itemInfo.render.rectTransform;
                        if (i == m_startIndex)
                        {
                            rectTransform.anchoredPosition = new Vector2(padding.left, -m_realPadding.top);
                        }
                        else
                        {
                            int index = i + 1;
                            ItemInfo tempItemInfo = m_itemInfos[i - 1];
                            RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                            if ((i + 1) % colCount == 1)
                                rectTransform.anchoredPosition = new Vector2(padding.left, tempRectTransform.anchoredPosition.y - tempItemInfo.size.y - gridSpacing.y);
                            else
                                rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + tempItemInfo.size.x + gridSpacing.x, tempRectTransform.anchoredPosition.y);
                        }
                    }
                    ItemInfo lastItemInfo = m_itemInfos[m_endIndex];
                    RectTransform lastRectTransform = lastItemInfo.render.rectTransform;

                    float width = colCount * lastItemInfo.size.x + (colCount - 1) * gridSpacing.x + m_realPadding.left + m_realPadding.right;//bIsMirror ? -lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_realPadding.right : lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_realPadding.right;
                    if (bIsAutoCenter)
                    {
                        float offset = width > viewRect.rect.width ? 0 : (viewRect.rect.width - width) * 0.5f;
                        if (offset > 0)
                        {
                            Vector2 offsetPos = new Vector2(offset, 0);
                            for (int i = m_startIndex; i <= m_endIndex; i++)
                            {
                                ItemInfo itemInfo = m_itemInfos[i];
                                RectTransform rectTransform = itemInfo.render.rectTransform;
                                rectTransform.anchoredPosition += offsetPos;
                            }
                            m_autoCenterPadding.left = m_autoCenterPadding.right = Mathf.RoundToInt(offset);
                            m_autoCenterPadding.top = m_autoCenterPadding.bottom = 0;
                        }
                    }
                    content.sizeDelta = new Vector2(content.sizeDelta.x, -lastRectTransform.anchoredPosition.y + lastItemInfo.size.y + m_realPadding.bottom);
                }
                break;
            case UIListViewLayout.GridHorizontal:
                {
                    for (int i = m_startIndex; i <= m_endIndex; i++)
                    {
                        ItemInfo itemInfo = m_itemInfos[i];
                        RectTransform rectTransform = itemInfo.render.rectTransform;
                        if (i == m_startIndex)
                        {
                            rectTransform.anchoredPosition = new Vector2(padding.left, -m_realPadding.top);
                        }
                        else
                        {
                            int index = i + 1;
                            ItemInfo tempItemInfo = m_itemInfos[i - 1];
                            RectTransform tempRectTransform = tempItemInfo.render.rectTransform;
                            if (i % colCount == 0)
                                rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + defSize.x + gridSpacing.x, -padding.top);
                            else
                                rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x, tempRectTransform.anchoredPosition.y - defSize.y - gridSpacing.y);
                        }
                    }
                    ItemInfo lastItemInfo = m_itemInfos[m_endIndex];
                    RectTransform lastRectTransform = lastItemInfo.render.rectTransform;
                    content.sizeDelta = new Vector2(lastRectTransform.anchoredPosition.x + lastItemInfo.size.x + m_realPadding.right, content.sizeDelta.y);
                }
                break;
        }
    }

    private void Cache(ItemInfo itemInfo)
    {
        UIListItemRender render = itemInfo.render;
        if (render != null)
        {
            render.gameObject.SetActive(false);
            m_renders.Add(render);
            itemInfo.render = null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Preview")]
    public void Preview()
    {
        if (Application.isPlaying || !content)
            return;

        layout = m_layout;
        Vector2 v2 = Vector2.zero;
        _defSize = null;

        switch (layout)
        {
            case UIListViewLayout.Vertical:
                v2 = bIsMirror ? new Vector2(0, 0) : new Vector2(0, 1);
                for (int i = 0; i < content.childCount; i++)
                {
                    RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                    rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = v2;
                    if (i == 0)
                        rectTransform.anchoredPosition = bIsMirror ? new Vector2(padding.left, padding.bottom) : new Vector2(padding.left, -padding.top);
                    else
                    {
                        RectTransform tempRectTransform = content.GetChild(i - 1).GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = bIsMirror ? new Vector2(padding.left, tempRectTransform.anchoredPosition.y + tempRectTransform.rect.size.y + spacing) : new Vector2(padding.left, tempRectTransform.anchoredPosition.y - tempRectTransform.rect.size.y - spacing);
                    }
                }
                if (bIsAutoCenter)
                {
                    RectTransform lastRectTransform = content.GetChild(content.childCount - 1).GetComponent<RectTransform>();
                    float height = bIsMirror ? lastRectTransform.anchoredPosition.y + lastRectTransform.rect.size.y + padding.top : -lastRectTransform.anchoredPosition.y + lastRectTransform.rect.size.y + padding.bottom;
                    float offset = height > viewRect.rect.height ? 0 : (viewRect.rect.height - height) * 0.5f;
                    if (offset > 0)
                    {
                        Vector2 offsetPos = new Vector2(0, bIsMirror ? -offset : offset);
                        for (int i = 0; i < content.childCount; i++)
                        {
                            RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                            rectTransform.anchoredPosition += offsetPos;
                        }
                    }
                }
                break;
            case UIListViewLayout.Horizontal:
                v2 = bIsMirror ? new Vector2(1, 1) : new Vector2(0, 1);
                for (int i = 0; i < content.childCount; i++)
                {
                    RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                    rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = v2;
                    if (i == 0)
                        rectTransform.anchoredPosition = bIsMirror ? new Vector2(-padding.right, -padding.top) : new Vector2(padding.left, -padding.top);
                    else
                    {
                        RectTransform tempRectTransform = content.GetChild(i - 1).GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = bIsMirror ? new Vector2(tempRectTransform.anchoredPosition.x - tempRectTransform.rect.size.x - spacing, -padding.top) : new Vector2(tempRectTransform.anchoredPosition.x + tempRectTransform.rect.size.x + spacing, -padding.top);
                    }
                }
                if (bIsAutoCenter)
                {
                    RectTransform lastRectTransform = content.GetChild(content.childCount - 1).GetComponent<RectTransform>();
                    float width = bIsMirror ? -lastRectTransform.anchoredPosition.x + lastRectTransform.rect.size.x + padding.left : lastRectTransform.anchoredPosition.x + lastRectTransform.rect.size.x + padding.right;
                    float offset = width > viewRect.rect.width ? 0 : (viewRect.rect.width - width) * 0.5f;
                    if (offset > 0)
                    {
                        Vector2 offsetPos = new Vector2(bIsMirror ? -offset : offset, 0);
                        for (int i = 0; i < content.childCount; i++)
                        {
                            RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                            rectTransform.anchoredPosition += offsetPos;
                        }
                    }
                }
                break;
            case UIListViewLayout.GridVertical:
                v2 = new Vector2(0, 1);
                if (bIsAutoColCount)
                    colCount = Mathf.FloorToInt((viewRect.rect.width + gridSpacing.x) / (defSize.x + gridSpacing.x));
                for (int i = 0; i < content.childCount; i++)
                {
                    RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                    rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = v2;
                    if (i == 0)
                        rectTransform.anchoredPosition = new Vector2(padding.left, -padding.top);
                    else
                    {
                        int index = i + 1;
                        RectTransform tempRectTransform = content.GetChild(i - 1).GetComponent<RectTransform>();
                        if ((i + 1) % colCount == 1)
                            rectTransform.anchoredPosition = new Vector2(padding.left, tempRectTransform.anchoredPosition.y - defSize.y - gridSpacing.y);
                        else
                            rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + defSize.x + gridSpacing.x, tempRectTransform.anchoredPosition.y);
                    }
                }
                break;
            case UIListViewLayout.GridHorizontal:
                v2 = new Vector2(0, 1);
                if (bIsAutoColCount)
                    colCount = Mathf.FloorToInt((viewRect.rect.height + gridSpacing.y) / (defSize.y + gridSpacing.y));
                for (int i = 0; i < content.childCount; i++)
                {
                    RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
                    rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = v2;
                    if (i == 0)
                        rectTransform.anchoredPosition = new Vector2(padding.left, -padding.top);
                    else
                    {
                        int index = i + 1;
                        RectTransform tempRectTransform = content.GetChild(i - 1).GetComponent<RectTransform>();
                        if (i % colCount == 0)
                            rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x + defSize.x + gridSpacing.x, -padding.top);
                        else
                            rectTransform.anchoredPosition = new Vector2(tempRectTransform.anchoredPosition.x, tempRectTransform.anchoredPosition.y - defSize.y - gridSpacing.y);
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
