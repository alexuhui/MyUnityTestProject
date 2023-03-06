using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RT : MonoBehaviour
{
    public RawImage[] RawImages;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Delay");
    }

    IEnumerator Delay()
    {
        rts = new RenderTexture[RawImages.Length];

        rts[0] = RenderTexture.GetTemporary(1920, 1080);
        RawImages[0].texture = rts[0];
        Camera.main.targetTexture = rts[0];
        yield return new WaitForSeconds(0.1f);


        rts[1] = new RenderTexture(1920, 1080, 16);
        RawImages[1].texture = rts[1];
        Camera.main.targetTexture = rts[1];
        yield return new WaitForSeconds(0.1f);

        rts[2] = RenderTexture.active;
        RawImages[2].texture = rts[2];
        Camera.main.targetTexture = rts[2];

        Camera.main.targetTexture = null;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool takePhoto = false;
    RenderTexture[] rts;
//    private void OnRenderImage(RenderTexture source, RenderTexture destination)
//    {
//        Graphics.Blit(source, destination);
//        if (takePhoto) return;
//        takePhoto = true;
//        Debug.Log($"---------------------- takePhoto {takePhoto}");
//        rts = new RenderTexture[RawImages.Length];

//        rts[0] = RenderTexture.GetTemporary(1920, 1080);
//#if DEBUG
//        rts[0].name = "rt 0";
//#endif
//        Graphics.Blit(source, rts[0]);
//        RawImages[0].texture = rts[0];
//        //rt1.Release();
//        //RenderTexture.ReleaseTemporary(rt1);


//        rts[1] = RenderTexture.GetTemporary(1920, 1080);
//        rts[1].name = "rt 1";
//        Graphics.Blit(source, rts[1]);
//        // rt2.Release();
//        RawImages[1].texture = rts[1];

//        rts[2] = RenderTexture.GetTemporary(1920, 1080);
//        rts[2].name = "rt 2";
//        Graphics.Blit(source, rts[2]);
//        RawImages[2].texture = rts[2];
//        //rt3.Release();
//        //Destroy(rt3);

//        rts[3] = new RenderTexture(1920, 1080, 16);
//        rts[3].name = "rt 3";
//        Graphics.Blit(source, rts[3]);
//        RawImages[3].texture = rts[3];
//    }

    public void OnGCClick()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
        Debug.Log("GC");
    }

    public void OnDestroyImageRootClick()
    {
        if (RawImages[0] != null)
            Destroy(RawImages[0].transform.parent.gameObject);
    }

    public void OnDestroyImageClick()
    {
        for (int i = 0; i < RawImages.Length; i++)
        {
            Destroy(RawImages[i]);
            rts[i]?.Release();
        }
        //RawImages = null;
        //rts = null;
        //this.enabled = false;
    }

    public void OnReleaseTempRTClick()
    {
        // 直接new的不能release
        for (int i = 0; i < rts.Length - 1; i++)
        {
            RenderTexture.ReleaseTemporary(rts[i]);
        }
       
    }
}
