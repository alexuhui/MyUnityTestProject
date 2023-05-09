using System;
using System.Collections;
using UnityEngine;

namespace HagoSpace.Svga
{
    public partial class SvgaPlayer
    {

#if UNITY_EDITOR
        // 编辑模式测试
        public int TestFrame;
        public bool AutoApply;

        [ContextMenu("NextFrame")]
        public void NextFrame()
        {
            TestFrame++;
            DrawFrame(TestFrame);
        }

        private void OnValidate()
        {
            if (m_SvgaData == null) return;
            TestFrame = Mathf.Max(Mathf.Min(TestFrame, m_SvgaData.TotalFrames-1), 0);
            if (AutoApply)
                DrawFrame(TestFrame);
        }
#endif



        private int m_CurrentFrame;

        private Coroutine m_TickCoroutine;
        private Action m_OnPlayComplete;

        /// <summary>
        /// 当前已播放次数.
        /// </summary>
        private int PlayedCount { get; set; }

        /// <summary>
        /// 播放循环次数, 默认为 0.
        /// 当为 0 时代表无限循环播放.
        /// </summary>
        private int LoopCount { get; set; }

        /// <summary>
        /// 是否处于播放状态
        /// </summary>
        private bool _isPlaying;


        private WaitForSeconds _WaitFrame;
        private WaitForSeconds m_WaitFrame {
            get { 
                if(_WaitFrame == null)
                    _WaitFrame = new WaitForSeconds(m_SvgaData.TimePerframe);
                return _WaitFrame;
            }
        }

        private void DrawFrame(int currentFrame)
        {
            var nodes = m_SvgaData.Nodes;
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                if(node == null)
                {
                    LogUtils.LogError($"DrawFrame node is null", Tag);
                    continue;
                }
                node.Node.DrawFrame(node.Frames[currentFrame]);
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="times"></param> 播放循环次数, 默认为 0.当为 0 时代表无限循环播放.
        /// <param name="callback"></param> 播放完成回调
        public void Play(int times = 0, Action callback = null)
        {
            // 禁止在播放状态下多次调用播放
            if (_isPlaying) return;

            m_OnPlayComplete = callback;
            PlayedCount = 0;
            LoopCount = times;
           
            Resume();
        }

        IEnumerator UpdateFrame()
        {
            while (PlayedCount < LoopCount || LoopCount == 0)
            {
                if(m_SvgaData == null)
                {
                    LogUtils.LogError("m_SvgaData is null", Tag);
                    yield break;
                }
                if (m_CurrentFrame > m_SvgaData.TotalFrames - 1)
                {
                    m_CurrentFrame = 0;
                    PlayedCount += 1;

                    if (PlayedCount >= LoopCount && LoopCount != 0)
                    {
                        PlayComplete();
                        yield break;
                    }
                }

                DrawFrame(m_CurrentFrame);
                m_CurrentFrame += 1;

                yield return m_WaitFrame;
            }
        }

        private void PlayComplete()
        {
            _isPlaying = false;
            m_OnPlayComplete?.Invoke();
        }

        public void Pause()
        {
            if(m_TickCoroutine != null)
            {
                StopCoroutine(m_TickCoroutine);
                m_TickCoroutine = null;
            }
                
            _isPlaying = false;
        }

        public void Resume()
        {
            _isPlaying = true;
            if(!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            m_TickCoroutine = StartCoroutine(UpdateFrame());
        }

        public void Stop()
        {
            Pause();
            m_CurrentFrame = 0;
        }
    }
}