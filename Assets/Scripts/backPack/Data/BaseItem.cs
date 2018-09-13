using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包物品基类
/// </summary>
public class BaseItem : MonoBehaviour {

    public int Id { get;  set; }
    public string Description { get;  set; }

    public Action<BaseItem> OnItemClickEvent { get; protected set; }

    protected object data;


    public BaseItem(int id, string description)
    {
        this.Id = id;
        this.Description = description;
    }
    public virtual void OnItemClick(GameObject go)
    {
        if (OnItemClickEvent != null)
        {
            OnItemClickEvent(this);
        }
    }

}
