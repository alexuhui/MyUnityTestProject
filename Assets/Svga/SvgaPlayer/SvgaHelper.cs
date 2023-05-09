
//#define UNITY_API 

using HagoSpace.Svga;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Svga;
using System.IO.Compression;

namespace HagoSpace
{
    public class SvgaHelper : MonoBehaviour
    {
        private const string Tag = "SvgaHelper";

        private static SvgaHelper m_Instance;
        public static SvgaHelper Instance {
            get
            {
                if (m_Instance == null)
                {
                    var go = new GameObject("SvgaHelper");
                    m_Instance = go.AddComponent<SvgaHelper>();
                    DontDestroyOnLoad(go);
                }
                return m_Instance;
            }
        }

        /// <summary> svga模板 </summary>
        private readonly Dictionary<string, GameObject> m_SvgaTemp = new Dictionary<string, GameObject>();
        /// <summary> svga缓存 </summary>
        private readonly Dictionary<string, List<GameObject>> m_SvgaCache = new Dictionary<string, List<GameObject>>();

        /// <summary> 正在加载的url </summary>
        private readonly HashSet<string> m_LoadingUrl = new HashSet<string>();
        /// <summary> 等待加载的url </summary>
        private readonly Dictionary<string, List<Action<string, SvgaPlayer>>> m_WaitingLoadAction = new Dictionary<string, List<Action<string, SvgaPlayer>>>();

        private WaitForSeconds m_WaitPreload = new WaitForSeconds(0.1f);
        private WaitForEndOfFrame m_WaitFrame = new WaitForEndOfFrame();

        private Matrix4x4 m_Matrix = new Matrix4x4(
                     new Vector4(0, 0, 0, 0),
                     new Vector4(0, 0, 0, 0),
                     new Vector4(0, 0, 0, 0),
                     new Vector4(0, 0, 0, 1)
                     );

        public void Clear()
        {
            StopAllCoroutines();

            foreach(var caches in m_SvgaCache)
            {
                foreach(var cache in caches.Value)
                {
                    LogUtils.LogInfo($"Destroy  cache : {caches.Key} --------------- ", Tag);
                    cache.GetComponent<SvgaPlayer>().Destroy();
                    Destroy(cache);
                }
            }
            m_SvgaCache.Clear();

            foreach(var temp in m_SvgaTemp)
            {
                if(temp.Value != null)
                {
                    LogUtils.LogInfo($"Destroy  temp : {temp.Key} --------------- ", Tag);
                    temp.Value.GetComponent<SvgaPlayer>().Destroy();
                    Destroy(temp.Value);
                }
                    
            }
            LogUtils.LogInfo($"Clear --------------- ", Tag);
            m_SvgaTemp.Clear();
            m_LoadingUrl.Clear();
            m_WaitingLoadAction.Clear();
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        public void Preload(string[] urls)
        {
            StartCoroutine(PreloadCor(urls));
        }

        private IEnumerator PreloadCor(string[] urls)
        {
            yield return new WaitForSeconds(3f);
            int index = 0;
            bool complete = false;
            bool isLoading = false;

            while(!complete)
            {
                if (!isLoading)
                {
                    isLoading = true;
                    if (index < 0 || index >= urls.Length)
                    {
                        complete = true;
                    }
                    else
                    {
                        var url = urls[index];
#if UNITY_EDITOR
                        LoadSvgaFromNet(url, (url, svga) =>
                        {
                            Cache(url, svga);
                            index++;
                            isLoading = false;
                        }, 0.5f);
#else
                        LoadSvgaFromApp(url, (url, svga) =>
                        {
                            Cache(url, svga);
                            index++;
                            isLoading = false;
                        }, 0.5f);
#endif
                    }
                }
                yield return m_WaitPreload;
            }
        }


        public void LoadSprite(string url, Action<Sprite> callback)
        {
           // StartCoroutine(ImageLoader.LoadSprite(url, callback));
        }

        public void LoadSvgaFromApp(string url, Action<string, SvgaPlayer> callback,float maxElapsed = 10)
        {
            LogUtils.LogInfo($"LoadSvgaFromApp --------------- url : {url}    callback : {callback}", Tag);

            // 从本地获取
            if(CheckLocal(url, out var local))
            {
                LogUtils.LogInfo($"LoadSvgaFromApp Get From Local : {local}   url : {url}", Tag);
                callback?.Invoke(url, local);
                return;
            }

            // 等待
            if(CheckLoading(url, callback))
            {
                return;
            }

            // 通过app获取
            GetSvgaFromApp(url, (localPath) =>
            {
                LogUtils.LogInfo($"LoadSvgaFromApp GetSvgaFromApp localPath: {localPath}   url : {url}", Tag);
                if (string.IsNullOrEmpty(localPath))
                {
                    callback?.Invoke(url, null);
                    return;
                }
                
                var bs = File.ReadAllBytes(localPath);
                if (bs == null)
                {
                    LogUtils.LogError($"LoadSvgaFromApp GetSvgaFromApp read bytes failed localPath : {localPath}", Tag);
                    callback?.Invoke(url, null);
                    return;
                }

                CreateSvga(url, bs, (svga) => {
                    callback?.Invoke(url, svga);
                }, maxElapsed);
            });
        }

        public void LoadSvgaFromNet(string url, Action<string, SvgaPlayer> callback, float maxElapsed = 10)
        {
            LogUtils.LogInfo($"LoadSvgaFromNet --------------- url : {url}    callback : {callback}", Tag);

            // 从本地获取
            if (CheckLocal(url, out var local))
            {
                LogUtils.LogInfo($"LoadSvgaFromNet Get From Local : {local}   url : {url}", Tag);
                callback?.Invoke(url, local);
                return;
            }

            // 等待
            if (CheckLoading(url, callback))
            {
                return;
            }

            // 从网络下载
            StartCoroutine(DownloadSvga(url, (bs) =>
            {
                LogUtils.LogInfo($"LoadSvgaFromNet DownloadSvga bs: {bs}   url : {url}", Tag);
                if (bs == null)
                {
                    LogUtils.LogError($"LoadSvgaFromNet DownloadSvga failed url : {url}", Tag);
                    callback?.Invoke(url, null);
                    return;
                }

                CreateSvga(url, bs, (svga) =>{
                    callback?.Invoke(url, svga);
                }, maxElapsed);
            }));
        }

        private SvgaPlayer CreateSvga(string url, byte[] bs, Action<SvgaPlayer> callback, float maxElapsed)
        {
            var svgaGo = new GameObject("SvgaPlayer", typeof(SvgaPlayer));
            svgaGo.layer = LayerMask.NameToLayer("UI");
            svgaGo.AddComponent<RectTransform>();
            var svga = svgaGo.GetComponent<SvgaPlayer>();
            using (Stream stream = new MemoryStream(bs))
            {
                StartCoroutine(LoadSvgaFileData(svga, stream, (svga) =>{                 
                    callback?.Invoke(svga);

                    m_SvgaTemp.Add(url, svgaGo);
                    // 处理等待任务
                    WaitingHandle(url);
                }, maxElapsed));
            }
            return svga;
        }


        public void Cache(string url, SvgaPlayer svga)
        {
            LogUtils.LogInfo($"Cache url : {url}   svga {svga}", Tag);
            if (svga == null)
            {
                LogUtils.LogError($"try to add an empty svga, url : {url}", Tag);
                return;
            }

            svga.Stop();
            svga.Hide();
            
            svga.transform.SetParent(transform, false);
            if (!m_SvgaCache.TryGetValue(url, out var caches))
            {
                caches = new List<GameObject>
                {
                    svga.gameObject
                };
                
                m_SvgaCache.Add(url, caches);
            }
            caches.Add(svga.gameObject);
        }

        private bool CheckLocal(string url, out SvgaPlayer svga)
        {
            // 从缓存获取
            if (CheckCache(url, out svga))
            {
                LogUtils.LogInfo($"LoadSvgaFromApp Get From Ceche : {svga}   url : {url}", Tag);
                return true;
            }

            // 从模板克隆
            if (CheckTemp(url, out svga))
            {
                LogUtils.LogInfo($"LoadSvgaFromApp Clone From Temp : {svga}   url : {url}", Tag);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测是否正在加载相同url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private bool CheckLoading(string url, Action<string, SvgaPlayer> callback)
        {
            LogUtils.LogInfo($"CheckLoading url : {url}", Tag);
            // 等待
            if (m_LoadingUrl.Contains(url))
            {
                if (!m_WaitingLoadAction.TryGetValue(url, out var list))
                {
                    list = new List<Action<string, SvgaPlayer>>();
                    m_WaitingLoadAction.Add(url, list);
                }
                list.Add(callback);
                LogUtils.LogInfo($"CheckLoading waiting loading url : {url}", Tag);
                return true;
            }

            m_LoadingUrl.Add(url);
            return false;
        }

        private void WaitingHandle(string url)
        {
            LogUtils.LogInfo($"WaitingHandle url : {url}", Tag);
            m_LoadingUrl.Remove(url);
            if (m_WaitingLoadAction.TryGetValue(url, out var list))
            {
                m_WaitingLoadAction.Remove(url);
                var temp = m_SvgaTemp[url];
                foreach (var item in list)
                {
                    SvgaPlayer svga = null;
                    if (temp == null)
                    {
                        LogUtils.LogError($"WaitingHandle  temp is null !   url : {url}");
                        
                    }else
                    {
                        svga = Clone(temp);
                    }
                    item?.Invoke(url, svga);
                }
            }
        }

        private bool CheckTemp(string url, out SvgaPlayer svga)
        {
            if (m_SvgaTemp.TryGetValue(url, out var temp))
            {
                LogUtils.LogInfo($"CheckTemp url : {url}  temp : {temp}", Tag);
                svga = Clone(temp);
                return true;
            }
            svga = null;
            return false;
        }

        private SvgaPlayer Clone(GameObject temp)
        {
            var go = Instantiate(temp);
            SvgaPlayer svga = go.GetComponent<SvgaPlayer>();
            svga.CloneData(temp.GetComponent<SvgaPlayer>());
            return svga;
        }

        private bool CheckCache(string url, out SvgaPlayer svga)
        {
            LogUtils.LogInfo($"CheckCache url : {url}", Tag);
            if (m_SvgaCache.TryGetValue(url, out var caches))
            {
                LogUtils.LogInfo($"CheckCache url : {url}  caches : {caches} cnt {caches?.Count}", Tag);
                if (caches != null && caches.Count > 0)
                {
                    int lastIndex = caches.Count - 1;
                    GameObject go = caches[lastIndex];
                    caches.RemoveAt(lastIndex);
                    svga = go.GetComponent<SvgaPlayer>();
                    return true;
                }
            }
            svga = null;
            return false;
        }

        private void GetSvgaFromApp(string url, Action<string> callback)
        {
            LogUtils.LogInfo($"GetSvgaFromApp url : {url}", Tag);
            
            //DressPvpDataProvider.DownloadSvga(url, callback);
        }

        private IEnumerator DownloadSvga(string url, Action<byte[]> callback)
        {
            LogUtils.LogInfo($"Download Svga url : {url}", Tag);
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
                {
                    LogUtils.LogError($"Download Svga Failed. url : {url}");
                    callback?.Invoke(null);
                }
                else
                {
                    var bs = www.downloadHandler.data;
                    LogUtils.LogInfo($"Download Svga success  url : {url}", Tag);
                    callback?.Invoke(bs);
                }
            }
        }


        /// <summary>
        /// 载入 SVGA 文件数据.
        /// </summary>
        /// <param name="svgaFileBuffer">SVGA 文件二进制 Stream.</param>
        public IEnumerator LoadSvgaFileData(SvgaPlayer svga, Stream svgaFileBuffer, Action<SvgaPlayer> callback, float maxElapsed)
        {
            if (svga == null || svgaFileBuffer == null)
            {
                LogUtils.LogError("LoadSvgaFileData error svga is null", Tag);
                callback?.Invoke(svga);
                yield break;
            }

            float start = Time.realtimeSinceStartup;

            byte[] inflatedBytes;
            // 微软自带的 DeflateStream 不认识文件头两个字节，SVGA 的这两字节为 78 9C，是 Deflate 的默认压缩表示字段.
            // 关于此问题请看 https://stackoverflow.com/questions/17212964/net-zlib-inflate-with-net-4-5.
            // Zlib 文件头请看 https://stackoverflow.com/questions/9050260/what-does-a-zlib-header-look-like.
            svgaFileBuffer.Seek(2, SeekOrigin.Begin);

            using (var deflatedStream = new DeflateStream(svgaFileBuffer, CompressionMode.Decompress))
            {
                using (var stream = new MemoryStream())
                {
                    deflatedStream.CopyTo(stream);
                    inflatedBytes = stream.ToArray();

                    if(Time.realtimeSinceStartup - start > maxElapsed)
                    {
                        start = Time.realtimeSinceStartup;
                        yield return m_WaitFrame;
                    }
                }
            }

            if (inflatedBytes == null)
            {
                LogUtils.LogError("LoadSvgaFileData error inflatedBytes is null", Tag);
                callback?.Invoke(svga);
                yield break;
            }

            var movieEntity = MovieEntity.Parser.ParseFrom(inflatedBytes);
            if (movieEntity == null)
            {
                LogUtils.LogError("LoadSvgaFileData error movieEntity is null", Tag);
                callback?.Invoke(svga);
                yield break;
            }
            var images = movieEntity.Images;
            
            if (images == null)
            {
                LogUtils.LogError("LoadSvgaFileData error images is null", Tag);
                callback?.Invoke(svga);
                yield break;
            }
            var imgDic = new Dictionary<string, Sprite>();

            foreach (var image in images)
            {
                if(string.IsNullOrEmpty(image.Key) || image.Value == null)
                {
                    LogUtils.LogError("LoadSvgaFileData error image is null", Tag);
                    continue;
                }

                var buffer = image.Value.ToByteArray();
                var tex = new Texture2D(1, 1);
                tex.LoadImage(buffer);
                var s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
                s.name = image.Key;

                imgDic.Add(image.Key, s);

                if (Time.realtimeSinceStartup - start > maxElapsed)
                {
                    start = Time.realtimeSinceStartup;
                    yield return m_WaitFrame;
                }
            }

            var sprites = movieEntity.Sprites;

            if(sprites == null)
            {
                callback?.Invoke(svga);
                LogUtils.LogError("LoadSvgaFileData error sprites is null", Tag);
                yield break;
            }

            if(movieEntity.Params == null)
            {
                callback?.Invoke(svga);
                LogUtils.LogError("LoadSvgaFileData error Params is null", Tag);
                yield break;

            }

            var svgaData = new SvgaData();
            svgaData.Width = movieEntity.Params.ViewBoxWidth;
            svgaData.Height = movieEntity.Params.ViewBoxHeight;
            svgaData.TimePerframe = 1f / movieEntity.Params.Fps;
            int totalFrams = movieEntity.Params.Frames;
            svgaData.TotalFrames = totalFrams;
            svgaData.Nodes = new SvgaNode[sprites.Count];

            var viewBox = new Vector2(svgaData.Width, svgaData.Height);
            if(svga != null)
            {
                var sr = svga.GetComponent<RectTransform>();
                if(sr != null)
                {
                    sr.sizeDelta = viewBox;
                }
            }
            

            bool getFirstLegalFrame;
            for (int i = 0; i < sprites.Count; i++)
            {
                var spriteData = sprites[i];
                if (spriteData == null || spriteData.Frames == null || spriteData.Frames.Count != totalFrams)
                {
                    LogUtils.LogError($"LoadSvgaFileData frame counts error   totalFrams {totalFrams}  count {spriteData?.Frames?.Count}", Tag);
                    continue;
                }

                var key = spriteData.ImageKey;
                // 创建节点实体
                var nodeObj = new GameObject(key, typeof(SvgaImgEx));
                nodeObj.layer = LayerMask.NameToLayer("UI");
                var img = nodeObj.GetComponent<SvgaImgEx>();
                nodeObj.transform.SetParent(svga.transform, false);

                if (!imgDic.TryGetValue(key, out var sprite))
                {
                    LogUtils.LogError($"empty svga node : {key}");
                }
                // 赋 Sprite 贴图
                img.sprite = sprite;
                // 设置初始布局
                var rect = nodeObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.sizeDelta = new Vector2(img.sprite.texture.width,
                    img.sprite.texture.height);
                img.raycastTarget = false;

                // 捆绑数据
                SvgaFrame[] frames = new SvgaFrame[totalFrams];
                getFirstLegalFrame = false;
                for (int f = 0; f < spriteData.Frames.Count; f++)
                {
                    var frameData = spriteData.Frames[f];
                    if(frameData == null ) {
                        LogUtils.LogError($"LoadSvgaFileData error frameData is null", Tag);
                        continue;
                    }

                    var frame = new SvgaFrame();
                    var result = GetFrame(frameData, out var pos, out var rot, out var scale, out var alpha);
                    frame.Alpha = alpha;
                    if (result || f == spriteData.Frames.Count - 1)
                    {
                        frame.Pos = pos;
                        frame.Rot = rot;
                        frame.Scale = scale;

                        if (!getFirstLegalFrame)
                        {
                            getFirstLegalFrame = true;
                            if (f > 0)
                            {
                                // 用第一帧有效数据填充前面的空帧
                                for (int pre = 0; pre < f; pre++)
                                {
                                    frames[pre].Pos = frame.Pos;
                                    frames[pre].Rot = rot;
                                    frames[pre].Scale = scale;
                                }
                            }
                        }
                    }
                    else if (getFirstLegalFrame)
                    {
                        //沿用上一帧数据
                        frame.Pos = frames[f - 1].Pos;
                        frame.Rot = frames[f - 1].Rot;
                        frame.Scale = frames[f - 1].Scale;
                    }
                    frames[f] = frame;
                }

                SvgaNode node = new SvgaNode();
                node.Key = key;
                node.Node = img;
                node.Frames = frames;
                svgaData.Nodes[i] = node;

                svga?.m_SubSpritesDic?.TryAdd(key, img);

                if (Time.realtimeSinceStartup - start > maxElapsed)
                {
                    start = Time.realtimeSinceStartup;
                    yield return m_WaitFrame;
                }
            }
            svga?.Init(svgaData);
            callback?.Invoke(svga);
        }

        private bool GetFrame(FrameEntity frameEntity, out Vector2 pos, out Quaternion rot, out Vector3 scale, out float alpha)
        {
            alpha = frameEntity.Alpha;
            var m = frameEntity.Transform;
            if (m != null)
            {
                m_Matrix.m00 = m.A; m_Matrix.m01 = -m.C;
                m_Matrix.m10 = -m.B; m_Matrix.m11 = m.D;

                pos = new Vector2(m.Tx, -m.Ty);
#if UNITY_API

                rot = m_Matrix.rotation;
                scale = m_Matrix.lossyScale;
#else
                //基向量
                Vector3 vecX = new Vector3(1, 0, 0);
                Vector3 vecY = new Vector3(0, 1, 0);
                //变换后基向量
                Vector3 vecTx = m_Matrix.MultiplyPoint(vecX);
                Vector3 vecTy = m_Matrix.MultiplyPoint(vecY);

                Debug.Log("==================================");
                Debug.Log($"vecTx {vecTx.magnitude}   vecTy  {vecTy.magnitude}");
                float scaleX = vecTx.magnitude, scaleY = vecTy.magnitude;

                //旋转
                var angleX = Vector2.Angle(vecX, vecTx);
                var angleY = Vector2.Angle(vecY, vecTy);
                var crossX = Vector3.Cross(vecX, vecTx);
                var crossY = Vector3.Cross(vecY, vecTy);

                float realAngle = 0;
                if (crossX.z == 0 && crossY.z == 0)
                {
                    scaleX = m.A;
                    scaleY = m.D;

                    if (scaleX < 0 && scaleY < 0)
                        realAngle = 180 - angleX;
                    else if (scaleX < 0)
                        realAngle = angleY;
                    else
                        realAngle = angleX;
                }
                else
                {
                    if (crossY.z * crossX.z < 0)
                    {
                        if (crossY.z < 0)
                            scaleY = -scaleY;
                        if (crossX.z < 0)
                            scaleX = -scaleX;
                    }

                    if (crossY.z < 0 && crossX.z < 0)
                        realAngle = 360 - angleX;
                    else if (crossX.z < 0)
                        realAngle = 180 - angleX;
                    else
                        realAngle = angleX;
                }

                Debug.Log($"angle {angleX}  crossX {crossX} crossY {crossY} realAngle  {realAngle}");

                scale = new Vector3(scaleX, scaleY, 1);
                rot = Quaternion.Euler(new Vector3(0, 0, realAngle));
#endif
                return true;
            }

            pos = Vector2.zero;
            rot = Quaternion.identity;
            scale = Vector3.one;
            return false;
        }
    }
}