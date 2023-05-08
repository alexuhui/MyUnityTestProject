using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using HagoSpace;

namespace HagoSpace
{
    public class ImageEx : Image
    {
        public SpriteAtlas Atlas;

        private Coroutine m_Coroutine;

        public string SpriteName
        {
            get
            {
                return sprite.name;
            }
            set
            {
                if (sprite.name == value)
                    return;
                SetSpriteByName(value);
            }
        }

        public virtual void SetSpriteByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("SetSpriteByName error : sprite name is empty or null");
                return;
            }

            if (Atlas == null)
            {
                Debug.LogError("SetSpriteByName error : atlas is null");
                return;
            }

            var sp = Atlas.GetSprite(name);
            if (sp == null)
            {
                Debug.LogError($"GetSprite error : Atlas {Atlas.name} not contain sprite {name}");
                return;
            }

            sprite = sp;
        }

        public virtual void SetSpriteByUrl(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                Debug.LogError("url is null");
                return;
            }

            //m_Coroutine = StartCoroutine(ImageLoader.LoadImage(this, url, rectTransform.rect.width, rectTransform.rect.height));
        }

        protected override void OnDestroy()
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);

            base.OnDestroy(); 
        }
    }
}
