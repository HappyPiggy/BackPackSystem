using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网格中item基类
/// </summary>
public abstract class BaseCell : MonoBehaviour {

    public int index;
    public object data;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    /// <summary>
    /// 重用时更新数据
    /// </summary>
    public virtual void UpdateInfo(int index,object data)
    {
        this.index = index;
        this.data = data;
    }

    public void SetAnchors(Vector2 min, Vector2 max)
    {
        if(rectTransform==null)
            rectTransform = GetComponent<RectTransform>();

        rectTransform.anchorMin = min;
        rectTransform.anchorMax = max;
    }


    public virtual float GetCellWidth()
    {
        return GetComponent<RectTransform>().sizeDelta.x;
    }
    public virtual float GetCellHeight()
    {
        return GetComponent<RectTransform>().sizeDelta.y;
    }
}
