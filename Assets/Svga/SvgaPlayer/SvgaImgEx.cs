using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Svga;
using System.Text;
using System;
using System.Net.NetworkInformation;

namespace HagoSpace.Svga
{
    public class SvgaImgEx : Image
    {
        private Color m_Color = new Color(1, 1, 1, 1);
        public void DrawFrame(SvgaFrame frame)
        {
            m_Color.a = frame.Alpha;
            color = m_Color;
            rectTransform.anchoredPosition = frame.Pos;
            rectTransform.rotation = frame.Rot;
            rectTransform.localScale = frame.Scale;
        }
    }
}