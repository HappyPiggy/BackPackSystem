using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour {

    private Text itemText;


	void Start () {
        itemText = transform.Find("Text").GetComponent<Text>() ;
    }


    /// <summary>
    /// 外部接口设置物品的text
    /// </summary>
    /// <param name="item"></param>
    public void UpdateItem(BaseItem item)
    {
        if(itemText==null)
            itemText = transform.Find("Text").GetComponent<Text>();

        itemText.text = item.Description + "\n" + item.Id;
    }
}
