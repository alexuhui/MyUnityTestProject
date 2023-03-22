using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListTestView : MonoBehaviour
{
    public UIListScrollRect listViewH;
    public UIListScrollRect listViewV;
    public UIListScrollRect listViewGH;
    public UIListScrollRect listViewGV;

    private void Start()
    {
        List<TestData> testDatas = new List<TestData>();

        for (int i = 0; i < 50; i++)
        {
            testDatas.Add(new TestData() { label = $"label {i}"});
        }


        for (int i = 0;i < testDatas.Count; i++)
        {
            listViewH.AddData(testDatas[i]);
            listViewV.AddData(testDatas[i]);
            listViewGH.AddData(testDatas[i]);
            listViewGV.AddData(testDatas[i]);
        }

        listViewH.SetSelect(0);
        listViewV.SetSelect(0);
        listViewGH.SetSelect(0);
        listViewGV.SetSelect(0);
    }
}
