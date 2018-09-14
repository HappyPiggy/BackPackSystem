using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网格中item基类
/// </summary>
public abstract class BaseCell : MonoBehaviour
{

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
    public virtual void UpdateInfo(int index, object data)
    {
        this.index = index;
        this.data = data;
    }



    public abstract float GetCellWidth();

    public abstract float GetCellHeight();
}

