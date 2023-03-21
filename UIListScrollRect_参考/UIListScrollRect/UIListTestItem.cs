using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIListTestView;

public class UIListTestItem : UIListItemRender
{
    public Text label;
    public override void SetData(object data)
    {
        base.SetData(data);
        label.text = ((TestData)data).label;
    }
}
