using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIListTestItem : UIListItemRender
{
    private TestData m_Data;

    public Text label;

    public override void OnDataRefresh()
    {
        base.OnDataRefresh();
        m_Data = GetData<TestData>();
        label.text = m_Data.label;
    }

    protected override void OnSelected(bool value)
    {
        base.OnSelected(value);
    }

    protected override void OnClick(PointerEventData data)
    {
        base.OnClick(data);

        Debug.Log($"OnClick index {Index}  label {m_Data.label}");
    }
}
