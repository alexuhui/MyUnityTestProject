using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UIEventListener : MonoBehaviour
{
    public delegate void PointerEventDelegate(PointerEventData eventData);
    public delegate void AxisEventDelegate(AxisEventData eventData);
    public delegate void BaseEventDelegate(BaseEventData eventData);
    public delegate void BaseEventPassDelegate(bool state);


    private PointerEventDelegate m_OnPointerEnter;

    public PointerEventDelegate OnPointerEnter
    {
        set
        {
            m_OnPointerEnter = value;
            GetOrCreateComponent<PointerEnterHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PointerEnterHandler>(gameObject).owner = this;
            return m_OnPointerEnter;
        }
    }


    private PointerEventDelegate m_OnPointerExit;

    public PointerEventDelegate OnPointerExit
    {
        set
        {
            m_OnPointerExit = value;
            GetOrCreateComponent<PointerExitHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PointerExitHandler>(gameObject).owner = this;
            return m_OnPointerExit;
        }
    }


    private PointerEventDelegate m_OnDrag;
    public PointerEventDelegate OnDrag
    {
        set
        {
            m_OnDrag = value;
            GetOrCreateComponent<DragHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<DragHandler>(gameObject).owner = this;
            return m_OnDrag;
        }
    }

    private PointerEventDelegate m_OnDrop;
    public PointerEventDelegate OnDrop
    {
        set
        {
            m_OnDrop = value;
            GetOrCreateComponent<DropHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<DropHandler>(gameObject).owner = this;
            return m_OnDrop;
        }
    }
    private PointerEventDelegate m_OnPointerDown;
    public PointerEventDelegate OnPointerDown
    {
        set
        {
            m_OnPointerDown = value;
            GetOrCreateComponent<PointerDownHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PointerDownHandler>(gameObject).owner = this;
            return m_OnPointerDown;
        }
    }
    private PointerEventDelegate m_OnPointerUp;
    public PointerEventDelegate OnPointerUp
    {
        set
        {
            m_OnPointerUp = value;
            GetOrCreateComponent<PointerUpHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PointerUpHandler>(gameObject).owner = this;
            return m_OnPointerUp;
        }
    }
    private PointerEventDelegate m_OnPointerClick;
    public PointerEventDelegate OnPointerClick
    {
        set
        {
            m_OnPointerClick = value;
            GetOrCreateComponent<PointerClickHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PointerClickHandler>(gameObject).owner = this;
            return m_OnPointerClick;
        }
    }

    private PointerEventDelegate m_OnPointerHoleClick;
    public PointerEventDelegate OnPointerHoleClick
    {
        set
        {
            m_OnPointerHoleClick = value;
            GetOrCreateComponent<PointerHoleClickHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PointerHoleClickHandler>(gameObject).owner = this;
            return m_OnPointerHoleClick;
        }
    }

    private Action<GameObject, int> m_OnDoubleClick;
    public Action<GameObject, int> OnDoubleClick
    {
        set
        {
            m_OnDoubleClick = value;
            GetOrCreateComponent<PointerDoubleClickHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PointerDoubleClickHandler>(gameObject).owner = this;
            return m_OnDoubleClick;
        }
    }

    private BaseEventDelegate m_OnSelect;
    public BaseEventDelegate OnSelect
    {
        set
        {
            m_OnSelect = value;
            GetOrCreateComponent<SelectHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<SelectHandler>(gameObject).owner = this;
            return m_OnSelect;
        }
    }
    private BaseEventDelegate m_OnDeselect;
    public BaseEventDelegate OnDeselect
    {
        set
        {
            m_OnDeselect = value;
            GetOrCreateComponent<DeselectHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<DeselectHandler>(gameObject).owner = this;
            return m_OnDeselect;
        }
    }
    private PointerEventDelegate m_OnScroll;
    public PointerEventDelegate OnScroll
    {
        set
        {
            m_OnScroll = value;
            GetOrCreateComponent<ScrollHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<ScrollHandler>(gameObject).owner = this;
            return m_OnScroll;
        }
    }
    private AxisEventDelegate m_OnMove;
    public AxisEventDelegate OnMove
    {
        set
        {
            m_OnMove = value;
            GetOrCreateComponent<MoveHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<MoveHandler>(gameObject).owner = this;
            return m_OnMove;
        }
    }
    private BaseEventDelegate m_OnUpdateSelected;
    public BaseEventDelegate OnUpdateSelected
    {
        set
        {
            m_OnUpdateSelected = value;
            GetOrCreateComponent<UpdateSelectedHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<UpdateSelectedHandler>(gameObject).owner = this;
            return m_OnUpdateSelected;
        }
    }
    private BaseEventDelegate m_OnSubmit;
    public BaseEventDelegate OnSubmit
    {
        set
        {
            m_OnSubmit = value;
            GetOrCreateComponent<SubmitHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<SubmitHandler>(gameObject).owner = this;
            return m_OnSubmit;
        }
    }
    private BaseEventDelegate m_OnCancel;
    public BaseEventDelegate OnCancel
    {
        set
        {
            m_OnCancel = value;
            GetOrCreateComponent<CancelHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<CancelHandler>(gameObject).owner = this;
            return m_OnCancel;
        }
    }
    private PointerEventDelegate m_OnInitializePotentialDrag;
    public PointerEventDelegate OnInitializePotentialDrag
    {
        set
        {
            m_OnInitializePotentialDrag = value;
            GetOrCreateComponent<InitializePotentialDragHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<InitializePotentialDragHandler>(gameObject).owner = this;
            return m_OnInitializePotentialDrag;
        }
    }
    private PointerEventDelegate m_OnBeginDrag;
    public PointerEventDelegate OnBeginDrag
    {
        set
        {
            m_OnBeginDrag = value;
            GetOrCreateComponent<BeginDragHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<BeginDragHandler>(gameObject).owner = this;
            return m_OnBeginDrag;
        }
    }
    private PointerEventDelegate m_OnEndDrag;
    public PointerEventDelegate OnEndDrag
    {
        set
        {
            m_OnEndDrag = value;
            GetOrCreateComponent<EndDragHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<EndDragHandler>(gameObject).owner = this;
            return m_OnEndDrag;
        }
    }

    private BaseEventDelegate m_OnPress;
    public BaseEventDelegate OnPress
    {
        set
        {
            m_OnPress = value;
            GetOrCreateComponent<PressHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PressHandler>(gameObject).owner = this;
            return m_OnPress;
        }
    }

    private BaseEventPassDelegate m_OnLongPress;
    public BaseEventPassDelegate OnLongPress
    {
        set
        {
            m_OnLongPress = value;
            GetOrCreateComponent<LongPressHandler>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<LongPressHandler>(gameObject).owner = this;
            return m_OnLongPress;
        }
    }

    private BaseEventPassDelegate m_OnPressEx;
    public BaseEventPassDelegate OnPressEx
    {
        set
        {
            m_OnPressEx = value;
            GetOrCreateComponent<PressHandlerEx>(gameObject).owner = this;
        }
        get
        {
            GetOrCreateComponent<PressHandlerEx>(gameObject).owner = this;
            return m_OnPressEx;
        }
    }



    public static UIEventListener Bind(GameObject obj)
    {
        UIEventListener listener = obj.GetComponent<UIEventListener>();
        if (listener == null)
            listener = obj.AddComponent<UIEventListener>();
        return listener;
    }

    public static void Clear(GameObject obj)
    {
        UIEventListener listener = obj.GetComponent<UIEventListener>();
        if (listener == null)
            return;

        listener.m_OnPointerEnter = null;
        listener.m_OnPointerExit = null;
        listener.m_OnDrag = null;
        listener.m_OnDrop = null;
        listener.m_OnPointerDown = null;
        listener.m_OnPointerUp = null;
        listener.m_OnPointerClick = null;
        listener.m_OnSelect = null;
        listener.m_OnDeselect = null;
        listener.m_OnScroll = null;
        listener.m_OnMove = null;
        listener.m_OnUpdateSelected = null;
        listener.m_OnInitializePotentialDrag = null;
        listener.m_OnBeginDrag = null;
        listener.m_OnEndDrag = null;
        listener.m_OnSubmit = null;
        listener.m_OnCancel = null;
        listener.m_OnPress = null;
    }

    public T GetOrCreateComponent<T>(GameObject go) where T : Component
    {
        T c = go.GetComponent<T>();
        if (c == null)
            c = go.AddComponent<T>();
        return c;
    }

    public virtual void OnDestroy()
    {
        m_OnPointerEnter = null;
        m_OnPointerExit = null;
        m_OnDrag = null;
        m_OnDrop = null;
        m_OnPointerDown = null;
        m_OnPointerUp = null;
        m_OnPointerClick = null;
        m_OnSelect = null;
        m_OnDeselect = null;
        m_OnScroll = null;
        m_OnMove = null;
        m_OnUpdateSelected = null;
        m_OnInitializePotentialDrag = null;
        m_OnBeginDrag = null;
        m_OnEndDrag = null;
        m_OnSubmit = null;
        m_OnCancel = null;
        m_OnPress = null;
    }



    public class PointerEnterHandler : MonoBehaviour, IPointerEnterHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (owner.OnPointerEnter != null)
                owner.OnPointerEnter(eventData);
        }
    }

  
    public class PointerExitHandler : MonoBehaviour, IPointerExitHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (owner.OnPointerExit != null)
                owner.OnPointerExit(eventData);
        }
    }

  
    public class PointerDownHandler : MonoBehaviour, IPointerDownHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (owner.OnPointerDown != null)
                owner.OnPointerDown(eventData);
        }
    }


  
    public class PointerUpHandler : MonoBehaviour, IPointerUpHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (owner.OnPointerUp != null)
                owner.OnPointerUp(eventData);
        }
    }

  
    public class PointerClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector]public UIEventListener owner;

        private void Awake()
        {

        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (owner.OnPointerClick != null)
                owner.OnPointerClick(eventData);
        }
    }

  
    public class PointerHoleClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector]public UIEventListener owner;
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (owner.OnPointerHoleClick != null)
                owner.OnPointerHoleClick(eventData);
            owner.HoleHandler(eventData, ExecuteEvents.pointerClickHandler);
        }
    }

  
    public class PointerDoubleClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector]public UIEventListener owner;
        public static float limitTime = 0.35f;
        private float countDown = 0;
        private int clickCnt = 0;
        PointerEventData eventData;


        public virtual void OnPointerClick(PointerEventData eventData)
        {
            this.eventData = eventData;
            countDown = limitTime;
            clickCnt += 1;
            if (clickCnt >= 2)
            {
                OnClickEnd();
            }
        }
        private void Update()
        {
            if (countDown > 0)
            {
                countDown -= Time.deltaTime;
                if (countDown <= 0)
                {
                    OnClickEnd();
                }
            }
        }
        void OnClickEnd()
        {
            if (owner.OnDoubleClick != null)
            {
                owner.OnDoubleClick(owner.gameObject, clickCnt);
            }
            clickCnt = 0;
            countDown = 0;
        }
    }

  
    public class InitializePotentialDragHandler : MonoBehaviour, IInitializePotentialDragHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (owner.OnInitializePotentialDrag != null)
                owner.OnInitializePotentialDrag(eventData);
        }
    }

  
    public class BeginDragHandler : MonoBehaviour, IBeginDragHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (owner.OnBeginDrag != null)
                owner.OnBeginDrag(eventData);
        }
    }

  
    public class EndDragHandler : MonoBehaviour, IEndDragHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (owner.OnEndDrag != null)
                owner.OnEndDrag(eventData);
        }
    }

  
    public class DragHandler : MonoBehaviour, IDragHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (owner.OnDrag != null)
                owner.OnDrag(eventData);
        }
    }

  
    public class DropHandler : MonoBehaviour, IDropHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (owner.OnDrop != null)
                owner.OnDrop(eventData);
        }
    }

  
    public class ScrollHandler : MonoBehaviour, IScrollHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnScroll(PointerEventData eventData)
        {
            if (owner.OnScroll != null)
                owner.OnScroll(eventData);
        }
    }

  
    public class UpdateSelectedHandler : MonoBehaviour, IUpdateSelectedHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            if (owner.OnUpdateSelected != null)
                owner.OnUpdateSelected(eventData);
        }
    }

  
    public class SelectHandler : MonoBehaviour, ISelectHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnSelect(BaseEventData eventData)
        {
            if (owner.OnSelect != null)
                owner.OnSelect(eventData);
        }
    }

  
    public class DeselectHandler : MonoBehaviour, IDeselectHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnDeselect(BaseEventData eventData)
        {
            if (owner.OnDeselect != null)
                owner.OnDeselect(eventData);
        }

    }

  
    public class MoveHandler : MonoBehaviour, IMoveHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnMove(AxisEventData eventData)
        {
            if (owner.OnMove != null)
                owner.OnMove(eventData);
        }

    }

  
    public class SubmitHandler : MonoBehaviour, ISubmitHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (owner.OnSubmit != null)
                owner.OnSubmit(eventData);
        }

    }

  
    public class CancelHandler : MonoBehaviour, ICancelHandler
    {
        [HideInInspector]public UIEventListener owner;

        public virtual void OnCancel(BaseEventData eventData)
        {
            if (owner.OnCancel != null)
                owner.OnCancel(eventData);
        }

    }

  
    public class PressHandler : MonoBehaviour, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        [HideInInspector]public UIEventListener owner;

        private bool isPress = false;
        private float downTime = 0;

        public void OnPointerDown(PointerEventData eventData)
        {
            downTime = Time.time;
            isPress = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPress = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPress = false;
        }

        private void FixedUpdate()
        {
            if (!isPress)
                return;

            if (Time.time - downTime > 0.5f)
            {
                isPress = false;
                if (owner.OnPress != null)
                    owner.OnPress(null);
            }
        }
    }

  
    public class LongPressHandler : MonoBehaviour, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        [HideInInspector]public UIEventListener owner;

        private bool isPress = false;
        private float downTime = 0;

        public void OnPointerDown(PointerEventData eventData)
        {
            downTime = Time.time;
            isPress = true;
            if (owner.OnLongPress != null)
                owner.OnLongPress(isPress);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPress = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPress = false;
            if (owner.OnLongPress != null)
                owner.OnLongPress(isPress);
        }

        private void FixedUpdate()
        {
            if (!isPress)
                return;

            if (Time.time - downTime > 0.5f)
            {
                if (owner.OnLongPress != null)
                    owner.OnLongPress(isPress);
            }
        }

        private void OnDisable()
        {
            isPress = false;
            if (owner.OnLongPress != null)
                owner.OnLongPress(isPress);
        }
    }

    // 穿透事件
    public void HoleHandler<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> eventFunction) where T : IEventSystemHandler
    {
        var current = ExecuteEvents.GetEventHandler<T>(eventData.pointerCurrentRaycast.gameObject);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var item in results)
        {
            var go = item.gameObject;
            if (go != null)
            {
                var excuteGo = ExecuteEvents.GetEventHandler<T>(go);

                // 没有注册或注册了其它类型句柄（比如mask）
                if (excuteGo == null)
                {
                    if (go.TryGetComponent<Graphic>(out var com))
                    {
                        if (com.raycastTarget) return;
                    }
                }
                else
                {
                    //注册句柄不是当前句柄则响应事件
                    if (excuteGo != current)
                    {
                        ExecuteEvents.Execute(excuteGo, eventData, eventFunction);
                        return;
                    }
                }
            }
        }
    }

  
    public class PressHandlerEx : MonoBehaviour, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        [HideInInspector]public UIEventListener owner;

        private bool isPress = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            isPress = true;
            if (owner.OnPressEx != null)
                owner.OnPressEx(isPress);
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            if (owner.OnPressEx != null && isPress)
            {
                isPress = false;
                owner.OnPressEx(isPress);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (owner.OnPressEx != null && isPress)
            {
                isPress = false;
                owner.OnPressEx(isPress);
            }
        }
    }
}
