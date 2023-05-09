using HagoSpace.Svga;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HagoSpace
{
    public class SvgaEx : MonoBehaviourEx
    {
        private enum Status
        {
            None,
            Play,
            Pause,
            Stop, 
        }

        private const string Tag = "SvgaEx";

        [HideInInspector]
        public SvgaPlayer Svga;
        public string Url;
        [Header("循环次数，0表示无限循环")]
        public int LoopTimes = 0;
        public bool AutoLoad = false;
        public bool AutoPlay = true;
        public bool AutoHide = false;

        private bool m_IsFirstLoad = true;
        private bool m_IsRecycle;
        private Action<object> m_OnPlayCompleteWithData;
        private Action m_OnPlayComplete;
        private object m_Data;
        private Status m_Status = Status.None;
        protected Dictionary<string, Sprite> m_ReplaceNodes;

        private RectTransform m_RectTransform;
        public RectTransform rectTransform
        {
            get
            {
                // The RectTransform is a required component that must not be destroyed. Based on this assumption, a
                // null-reference check is sufficient.
                if (ReferenceEquals(m_RectTransform, null))
                {
                    m_RectTransform = GetComponent<RectTransform>();
                }
                return m_RectTransform;
            }
        }


        private void Awake()
        {
            m_IsRecycle = false;

            if (AutoLoad)
            {
                Load(Url);
            }
        }

        private void OnDestroy()
        {
            LogUtils.LogInfo($"OnDestroy --------------- url : {Url}", Tag);
            Recycle();
        }

        public void SetPlayCompleteCallback(Action<object> onComplete)
        {
            m_OnPlayCompleteWithData = onComplete;
        }
        public void SetPlayCompleteCallback(Action onComplete)
        {
            m_OnPlayComplete = onComplete;
        }

        public void Load (string url, int loop, bool autoPlay, bool autoHide, object data = null)
        {
            m_Data = data;
            AutoLoad = false;
            LoopTimes = loop;
            AutoPlay = autoPlay;
            AutoHide = autoHide;
            Load(url);
        }

        public void Load(string url)
        {
#if UNITY_EDITOR
            LoadFromNet(url);
#else
            LoadFromApp(url);
#endif
        }

        public void Play()
        {
            m_Status =  Status.Play;
            if(Svga != null)
            {
                Svga.Play(LoopTimes, OnPlayComplete);
            }
        }

        public void Pause()
        {
            m_Status = Status.Pause;
            if (Svga != null)
            {
                Svga.Pause();
            }
        }

        public void Stop()
        {
            m_Status = Status.Stop;
            if (Svga != null)
            {
                Svga.Stop();
            }
        }

        public void LoadFromNet(string url, int loop, bool autoPlay, bool autoHide)
        {
            LoopTimes = loop;
            AutoPlay = autoPlay;
            AutoHide = autoHide;
            LoadFromNet(url);
        }

        public void LoadFromNet(string url)
        {
            LogUtils.LogInfo($"LoadFromNet  url : {url}  LoopTimes {LoopTimes}  AutoPlay {AutoPlay}  AutoHide {AutoHide}", Tag);
            if (string.IsNullOrEmpty(url) || CheckSame(url)) return;

            SvgaHelper.Instance.LoadSvgaFromNet(url, LoadComplete);
        }

        public void LoadFromApp(string url, int loop, bool autoPlay, bool autoHide)
        {
            LoopTimes = loop;
            AutoPlay = autoPlay;
            AutoHide = autoHide;
            LoadFromApp(url);
        }

        public void LoadFromApp(string url)
        {
            LogUtils.LogInfo($"LoadFromApp  url : {url}  LoopTimes {LoopTimes}  AutoPlay {AutoPlay}  AutoHide {AutoHide}", Tag);
            if (string.IsNullOrEmpty(url) || CheckSame(url)) return;

            SvgaHelper.Instance.LoadSvgaFromApp(url, LoadComplete);
        }

        private bool CheckSame(string url)
        {
            LogUtils.LogInfo($"CheckSame  url : {url}  oldUrl : {Url}   Svga : {Svga}  ", Tag);
            if (Svga != null)
            {
                if (Url == url)
                {
                    LoadComplete(url, Svga);
                    return true;
                }
                LogUtils.LogInfo($"CheckSame  Cache  oldUrl : {Url}   Svga : {Svga}  ", Tag);
                SvgaHelper.Instance.Cache(Url, Svga);
                Svga = null;
            }
            else if(Url == url && !m_IsFirstLoad)
            {
                return true;
            }
            m_IsFirstLoad = false;

            // url 被替换了，清除待替换节点
            if (m_ReplaceNodes != null)
            {
                m_ReplaceNodes.Clear();
            }
            Url = url;
            return false;
        }

        public void Recycle()
        {
            LogUtils.LogInfo($"Recycle oldUrl : {Url}   Svga : {Svga}  ", Tag);
            if (Svga != null)
            {
                SvgaHelper.Instance.Cache(Url, Svga);
            }
            Svga = null;
            m_IsRecycle = true;
            m_IsFirstLoad = true;
        }

        public void ReplaceNode(string node, Sprite sprite)
        {
            LogUtils.LogInfo($"ReplaceNode  node : {node}  sprite : {sprite}", Tag);
            if(Svga != null)
            {
                Svga.ReplaceNode(node, sprite);
            }else
            {
                if (m_ReplaceNodes == null)
                    m_ReplaceNodes = new Dictionary<string, Sprite>();

                if (m_ReplaceNodes.ContainsKey(node))
                {
                    m_ReplaceNodes[node] = sprite;
                }
                else
                {
                    m_ReplaceNodes.Add(node, sprite);
                }
            }
        }

        public void ReplaceNode(string node, string url)
        {
            LogUtils.LogInfo($"ReplaceNode  node : {node}  url : {url}", Tag);
            SvgaHelper.Instance.LoadSprite(url, (sprite) => {
                ReplaceNode(node, sprite);
            });
        }

        private void LoadComplete(string url, SvgaPlayer svga)
        {
            LogUtils.LogInfo($"LoadComplete  url : {url}    svga : {svga}", Tag);
            if (svga == null)
            {
                LogUtils.LogError($"LoadComplete  svga is null url : {url}", Tag);
                return;
            }

            if(m_IsRecycle || url != Url)
            {
                LogUtils.LogInfo($"LoadComplete m_IsRecycle url : {url}", Tag);
                SvgaHelper.Instance.Cache(url, Svga);
                return;
            }

            Svga = svga;
            var sTran = Svga.transform;
            sTran.SetParent(transform, false);
            sTran.localPosition = Vector3.zero;

            if(m_ReplaceNodes!= null)
            {
                foreach(var node in m_ReplaceNodes)
                {
                    Svga.ReplaceNode(node.Key, node.Value);
                }
                m_ReplaceNodes.Clear();
            }

            if(m_Status == Status.Stop)
            {
                Svga.Stop();
            }else if (m_Status == Status.Pause) {
                Svga.Pause();
            }
            if (m_Status == Status.Play || AutoPlay)
            {
                LogUtils.LogInfo($"LoadComplete AutoPlay LoopTimes : {LoopTimes}", Tag);
                Play();
            }else
            {
                Svga.Hide();
            }
        }

        private void OnPlayComplete()
        {
            LogUtils.LogInfo($"OnPlayComplete AutoHide : {AutoHide}", Tag);
            m_OnPlayCompleteWithData?.Invoke(m_Data);
            m_OnPlayComplete?.Invoke();
            if (AutoHide)
            {
                m_Status = Status.None;
                Svga.Stop();
                Svga.Hide();
            }
        }
    }
}