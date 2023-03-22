using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIListItemRender : UIBehaviour
{
    public int Index;
    public bool CanClick = true;
    public GameObject SelectedShowObj;
    public GameObject UnselectedShowObj;
    public Action OnRectSizeChange;

    private UIListScrollRect m_ListScrollRect;
    private ItemDataBase m_Data;
    private UIListItemSelectStatus selectedStatus = UIListItemSelectStatus.None;

    private RectTransform m_RectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = GetComponent<RectTransform>();
            }
            return m_RectTransform;
        }
    }

    protected override void Awake()
    {
        if (CanClick)
        {
            UIEventListener.Bind(gameObject).OnPointerClick = OnClick;
        }
    }

    protected override void OnDestroy()
    {
        UIEventListener.Clear(gameObject);
        m_Data = null;
    }

    public void SetData(ItemDataBase data, int index)
    {
        m_Data = data;
        Index = index;
        OnDataRefresh();
    }

    public virtual void OnDataRefresh()
    {
    }

    public T GetData<T>() where T : ItemDataBase 
    {
        if (!(m_Data is T))
            throw new Exception($"数据类型 {m_Data.GetType()} 不匹配 {typeof(T)}");

        return m_Data as T;
    }

    protected virtual void OnClick(GameObject go, PointerEventData data)
    {
        m_ListScrollRect.SetSelect(Index);
    }

    public void SetSelected(UIListItemSelectStatus status)
    {
        if (status == selectedStatus) return;

        if (status != UIListItemSelectStatus.None)
            OnSelected(UIListItemSelectStatus.Selected == status);
    }

    protected virtual void OnSelected(bool value)
    {
        if (SelectedShowObj != null)
            SelectedShowObj?.SetActive(value);
        if(UnselectedShowObj != null)
            UnselectedShowObj?.SetActive(!value);
    }

    public virtual UIListItemRender Clone(UIListScrollRect scrollRect)
    {
        var obj = Instantiate(gameObject);
        UIListItemRender item = obj.GetComponent<UIListItemRender>();
        item.m_ListScrollRect = scrollRect;
        return item;
    }

    //RectTransform 关联尺寸改变
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        if (OnRectSizeChange != null)
            OnRectSizeChange();
    }
}
