using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Svga;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Networking;

namespace HagoSpace.Svga
{
    public partial class SvgaPlayer : MonoBehaviour
    {
        private const string Tag = "SvgaPlayer";
        public readonly Dictionary<string, SvgaImgEx> m_SubSpritesDic= new Dictionary<string, SvgaImgEx>();

        public void CloneData(SvgaPlayer other)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var imgEx = child.GetComponent<SvgaImgEx>();
                var key = child.name;
                m_SubSpritesDic.TryAdd(key, imgEx);
            }
        }

        public void ReplaceNode(string node, Sprite sprite)
        {
            LogUtils.LogInfo($"ReplaceNode  node {node}   sprite {sprite?.name}", Tag);
            foreach(var n in m_SubSpritesDic)
            {
                LogUtils.LogInfo($"ReplaceNode m_SubSpritesDic node {n.Key}   SvgaImgEx {n.Value}", Tag);
            }
            if (m_SubSpritesDic.TryGetValue(node, out SvgaImgEx img))
            {
                LogUtils.LogInfo($"ReplaceNode  node {node} img  {img}  sprite {sprite?.name}", Tag);
                img.sprite = sprite;
            }
        }

        //public void ParseBytes(byte[] bs, Action<SvgaPlayer> callback)
        //{
        //    using (Stream m_Stream = new MemoryStream(bs))
        //    {
        //        LoadSvgaFileData(m_Stream);
        //        InitialView();
        //    }
        //}

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}