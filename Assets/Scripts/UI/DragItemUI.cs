using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拖拽时跟随鼠标的物体
/// </summary>
public class DragItemUI : ItemUI {
    
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void UpdatePosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }
}
