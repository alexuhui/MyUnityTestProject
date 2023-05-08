using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;

public class AtlasTest : MonoBehaviour
{

    public Image Image;

    private SpriteAtlas m_Atlas1;
    private SpriteAtlas m_Atlas2;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        AssetBundle ab1 = AssetBundle.LoadFromFile("Assets/StreamingAssets/Atlas1");
        m_Atlas1 = ab1.LoadAsset<SpriteAtlas>("Atlas1.spriteatlas");

        AssetBundle ab2 = AssetBundle.LoadFromFile("Assets/StreamingAssets/Atlas2");
        m_Atlas2 = ab2.LoadAsset<SpriteAtlas>("Atlas2.spriteatlas");

        Image.sprite = ab1.LoadAsset<Sprite>("Atlas1/bg_btn.png");
    }

}
