using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIListTestView : MonoBehaviour
{
    public UIListScrollRect listViewH;
    public UIListScrollRect listViewV;
    public UIListScrollRect listViewGH;
    public UIListScrollRect listViewGV;

    [SerializeField] private InputField inputJump;
    [SerializeField] private GameObject btnJump;

    private void Awake()
    {
        UIEventListener.Bind(btnJump).OnPointerClick = OnBtnJumpClick;
    }
    private void OnDestroy()
    {
        UIEventListener.Clear(btnJump);
    }


    private void OnBtnJumpClick(GameObject go, PointerEventData data)
    {
        if(int.TryParse(inputJump.text, out int index))
        {
            listViewH.ScrollToItem(index);
            listViewV.ScrollToItem(index);
            listViewGH.ScrollToItem(index);
            listViewGV.ScrollToItem(index);
        }
    }

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
