using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIListTestView;

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
}
