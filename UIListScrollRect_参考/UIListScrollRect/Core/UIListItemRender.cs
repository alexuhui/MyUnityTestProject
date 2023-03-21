using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIListItemRender : UIBehaviour
{
    [NonSerialized]
    public int m_index;

    public Action onRectSizeChange;

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

    public object data { get; private set; }

    public int index { get { return m_index; } }

    public bool bInit { get; protected set; }

    public virtual void SetData(object data)
    {
        this.data = data;
    }

    public virtual void SetSelected(bool value)
    {

    }

    protected override void OnDestroy()
    {
        this.data = null;
    }

    public virtual UIListItemRender Clone(bool bInit = true)
    {
        var obj = GameObject.Instantiate(gameObject);
        UIListItemRender item = obj.GetComponent<UIListItemRender>();
        if (bInit)
            Init(item);
       
        return item;
    }

    public virtual void Init(UIListItemRender item)
    {
        item.bInit = true;
    }

    public void TryInit()
    {
        if (!bInit)
            Init(this);
    }


    //RectTransform 关联尺寸改变
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        if (onRectSizeChange != null)
            onRectSizeChange();
    }
}
