using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PlasticGui.PlasticTableColumn;

public class UIListCreator
{
    private const string m_MenuRoot = "GameObject/UI/ScrollRect/";

    [MenuItem(m_MenuRoot + "Vertical")]
    private static void CreateVertical()
    {
        var scrollrect = CreateUIListScrollRect();
        scrollrect.Layout = UIListViewLayout.Vertical;
    }

    [MenuItem(m_MenuRoot + "Horizontal")]
    private static void CreateHorizontal()
    {
        var scrollrect = CreateUIListScrollRect();
        scrollrect.Layout = UIListViewLayout.Horizontal;
    }

    [MenuItem(m_MenuRoot + "GridVertical")]
    private static void CreateGridVertical()
    {
        var scrollrect = CreateUIListScrollRect();
        scrollrect.Layout = UIListViewLayout.GridVertical;
    }

    [MenuItem(m_MenuRoot + "GridHorizontal")]
    private static void CreateGridHorizontal()
    {
        var scrollrect = CreateUIListScrollRect();
        scrollrect.Layout = UIListViewLayout.GridHorizontal;
    }


    private static UIListScrollRect CreateUIListScrollRect()
    {
        Transform parent = Selection.activeTransform;
        GameObject go = new GameObject("UIListScrollRect");
        RectTransform rectTran = go.AddComponent<RectTransform>();
        UIListScrollRect scrollrect = go.AddComponent<UIListScrollRect>();
        Image bg = go.AddComponent<Image>();
        if (parent != null)
        {
            rectTran.SetParent(parent, false);
        }
        rectTran.sizeDelta = new Vector2(200, 200);


        GameObject viewGo = new GameObject("Viewport");
        RectTransform viewTran = viewGo.AddComponent<RectTransform>();
        viewGo.AddComponent<Image>();
        Mask mask = viewGo.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        viewTran.SetParent(rectTran, false);
        viewTran.pivot = new Vector2(0,1);
        viewTran.anchorMin = Vector2.zero;
        viewTran.anchorMax = Vector2.one;
        viewTran.offsetMin = new Vector2(15, 15);
        viewTran.offsetMax = new Vector2(-15, -15);
        scrollrect.viewport = viewTran;


        GameObject contentGo = new GameObject("Content");
        RectTransform contentTran = contentGo.AddComponent<RectTransform>();
        contentTran.SetParent(viewTran, false);
        scrollrect.content = contentTran;
        contentTran.anchorMin = new Vector2(0, 1);
        contentTran.anchorMax = Vector2.one;
        contentTran.pivot = new Vector2(0, 1);
        contentTran.sizeDelta = new Vector2(0, 300);

        return scrollrect;
    }
}
