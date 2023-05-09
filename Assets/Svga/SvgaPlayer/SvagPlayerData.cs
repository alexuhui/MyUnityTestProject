using UnityEngine;

namespace HagoSpace.Svga
{
    // svga proto 地址 https://github.com/svga/SVGA-Format/blob/master/proto/svga.proto

    [System.Serializable]
    public class SvgaFrame
    {
        public float Alpha;
        public Vector2 Pos;
        public Quaternion Rot;
        public Vector3 Scale;
    }

    [System.Serializable]
    public class SvgaNode
    {
        public string Key;
        public SvgaImgEx Node;
        public SvgaFrame[] Frames;
    }

    [System.Serializable]
    public class SvgaData
    {
        public float Width;
        public float Height;
        public float TimePerframe;
        public int TotalFrames;

        public SvgaNode[] Nodes;
    }

    public partial class SvgaPlayer
    {
        [SerializeField]
        private SvgaData m_SvgaData;

        
        public void Init(SvgaData data)
        {
            m_SvgaData = data;
        }

        public void Destroy()
        {
            StopAllCoroutines();
            if(m_SvgaData != null && m_SvgaData.Nodes != null)
            {
                for (int i = 0; i < m_SvgaData.Nodes.Length; i++)
                {
                    var node = m_SvgaData.Nodes[i];
                    if(node != null && node.Node != null) {
                        Destroy(node.Node.gameObject);
                    }
                }
            }
            
            m_SvgaData = null;
        }

    }
}