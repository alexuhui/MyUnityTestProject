using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIListItemRender : UIBehaviour
{
    public Action OnRectSizeChange;

    
    private RectTransform m_RectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (ReferenceEquals(m_RectTransform, null))
            {
                m_RectTransform = GetComponent<RectTransform>();
            }
            return m_RectTransform;
        }
    }

    private ItemDataBase m_Data;

    private int m_Index;
    public int Index { get { return m_Index; } }

    public void SetData(ItemDataBase data)
    {
        this.m_Data = data;
        OnDataRefresh();
    }

    public virtual void OnDataRefresh()
    {
    }

    public T GetData<T>() where T : ItemDataBase 
    {
        if (!(this.m_Data is T))
            throw new Exception($"数据类型 {m_Data.GetType()} 不匹配 {typeof(T)}");

        return this.m_Data as T;
    }

    public virtual void SetSelected(bool value)
    {
        //Debug.Log($"set select status : index  {Index}  status  {value}");
    }

    protected override void OnDestroy()
    {
        this.m_Data = null;
    }

    public virtual UIListItemRender Clone()
    {
        var obj = GameObject.Instantiate(gameObject);
        UIListItemRender item = obj.GetComponent<UIListItemRender>();

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
