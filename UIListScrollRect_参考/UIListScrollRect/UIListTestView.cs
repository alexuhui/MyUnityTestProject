using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListTestView : MonoBehaviour
{
    public UIListScrollRect listView;

    public class TestData
    {
        public string label;
    }

    private void Start()
    {
        List<TestData> testDatas = new List<TestData>() {
            new TestData(){ label = "label 01"},
            new TestData(){ label = "label 02"},
            new TestData(){ label = "label 03"},
            new TestData(){ label = "label 04"},
            new TestData(){ label = "label 05"},
        };

        for (int i = 0;i < testDatas.Count; i++)
        {
            listView.AddData(testDatas[i]);
        }
    }
}
