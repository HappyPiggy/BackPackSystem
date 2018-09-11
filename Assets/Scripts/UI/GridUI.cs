using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridUI : MonoBehaviour, IBeginDragHandler,IEndDragHandler,IDragHandler
{
    public static Action<Transform> OnBeginDragEvent;
    public static Action<Transform,Transform> OnEndDragEvent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button== PointerEventData.InputButton.Left)
        {
            if (OnBeginDragEvent != null)
            {
                OnBeginDragEvent(transform);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (OnEndDragEvent != null)
            {
                if (eventData.pointerEnter == null)
                    OnEndDragEvent(transform,null); //丢掉item
                else
                    OnEndDragEvent(transform, eventData.pointerEnter.transform); //换位置
            }
        }
    }


}
