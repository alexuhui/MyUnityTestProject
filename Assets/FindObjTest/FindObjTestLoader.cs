using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class FindObjTestLoader : MonoBehaviour
{
    AsyncOperation asyncOper;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        asyncOper = SceneManager.LoadSceneAsync(@"FindObjTest");
    }

    bool done = false;
    GameObject birthPos;
    // Update is called once per frame
    void Update()
    {
        if (done) return;

        if(asyncOper.isDone)
        {
            done = true;
            Debug.Log("load scene done");

            var mainScene = SceneManager.GetSceneByName("FindObjTest");
            var allobj = mainScene.GetRootGameObjects();
            for (int i = 0; i < allobj.Length; i++)
            {
                Debug.Log(allobj[i].name);

                if (allobj[i].name == "bornPos")
                {
                    birthPos = allobj[i].transform.Find("default").gameObject;
                }
            }
            Debug.Log(birthPos.name);
        }

        
    }
}
