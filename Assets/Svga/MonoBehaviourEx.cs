using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HagoSpace
{
    public class MonoBehaviourEx : MonoBehaviour
    {
        bool _init = false;
        private Transform m_CachedTransform;
        public Transform CachedTransform
        {
            get
            {
                if (!_init)
                {
                    _init = true;
                    m_CachedTransform = transform;
                }
                return m_CachedTransform;
            }
        }
    }
}
