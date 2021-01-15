using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ObjectClick : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public UnityEvent onclick = new UnityEvent();
    public UnityEvent onEnter = new UnityEvent();
    public UnityEvent onExit = new UnityEvent();
    /// <summary>
    /// 封装物体点击事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        onclick.Invoke();
    }

    /// <summary>
    /// 进入
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter.Invoke();
    }

    /// <summary>
    /// 退出
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        onExit.Invoke();
    }
}
