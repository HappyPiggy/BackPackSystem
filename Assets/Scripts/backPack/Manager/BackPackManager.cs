using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPackManager : MonoBehaviour {

    private static BackPackManager _instance;
    public static BackPackManager Instance
    {
        get;set;
    }


    private Dictionary<int,BaseItem> itemDic;//储存所有道具的字典
    private Dictionary<Transform,BaseItem> packPackItemDic;//储存现在背包上道具的字典

    private GridPanelUI gridPanelUI;
    private DragItemUI dragItemUI;

    private bool isDrag = false;

    private void Start()
    {
        _instance = this;
        gridPanelUI = GetComponentInChildren<GridPanelUI>();

        GridUI.OnBeginDragEvent += OnBeginDragEvent;
        GridUI.OnEndDragEvent += OnEndDragEvent;

        packPackItemDic = new Dictionary<Transform, BaseItem>();
        Load();
    }


    private  void OnBeginDragEvent(Transform transform)
    {
        //空格子不处理
        if (transform.childCount == 0)
            return;

        if (packPackItemDic.ContainsKey(transform))
        {
            BaseItem item = packPackItemDic[transform];
            dragItemUI.UpdateItem(item);
            Destroy(transform.GetChild(0).gameObject);

            dragItemUI.Show();
            isDrag = true;
        }
    }

    private void OnEndDragEvent(Transform preTransform, Transform curTransform)
    {
        //丢掉物品
        if (curTransform == null)
        {
            packPackItemDic.Remove(preTransform);
        }
        else if(curTransform.CompareTag("grid"))//交换物品
        {
            if (curTransform.childCount == 0)
            {
                //拖拽到空格子
                if (packPackItemDic.ContainsKey(preTransform))
                {
                    BaseItem item = packPackItemDic[preTransform];
                    packPackItemDic.Remove(preTransform);
                    CreateItem(item, curTransform);
                }

            }
            else
            {
                //交换两个格子item
                if (packPackItemDic.ContainsKey(preTransform) && packPackItemDic.ContainsKey(curTransform))
                {
                    Destroy(curTransform.GetChild(0).gameObject);

                    BaseItem preItem = packPackItemDic[preTransform];
                    BaseItem curItem = packPackItemDic[curTransform];

                    CreateItem(preItem, curTransform);
                    CreateItem(curItem, preTransform);
                }
            }
        }
        else //回到原位
        {
            if (packPackItemDic.ContainsKey(preTransform))
            {
                BaseItem item = packPackItemDic[preTransform];
                CreateItem(item, preTransform);
            }
        }


        dragItemUI.Hide();
        isDrag = false;
    }


    /// <summary>
    /// 增加对应idx的物品
    /// </summary>
    /// <param name="itemIndex"></param>
    public void AddItem(int itemIndex)
    {
        if (!itemDic.ContainsKey(itemIndex))
            return;

        BaseItem item = itemDic[itemIndex];
        var grid = gridPanelUI.GetUsableGrid();

        if (grid == null)
        {
            Debug.Log("无多余空间");
            return;
        }

        CreateItem(item,grid);
    }

    /// <summary>
    /// 创建新item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="parent"></param>
    private void CreateItem(BaseItem item,Transform parent)
    {
        var res = Resources.Load("backPack/item");
        GameObject itemObj = GameObject.Instantiate(res) as GameObject;

        var itemUI = itemObj.GetComponent<ItemUI>();
        itemUI.UpdateItem(item);

        itemObj.transform.SetParent(parent);
        itemObj.transform.localPosition = Vector3.zero;
        itemObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

        packPackItemDic[parent] = item;
    }


    /// <summary>
    /// 配置表或者数据库加载所有道具到list
    /// 加载draguiItem
    /// </summary>
    private void Load()
    {
        itemDic = new Dictionary<int, BaseItem>();
        var item1 = new SkillItem(0,"技能1",10);
        var item2 = new WeaponItem(1,"武器1",99);
        var item3 = new WeaponItem(3,"武器2",99);
        var item4 = new WeaponItem(4, "技能2", 99);
        itemDic[item1.Id] = item1;
        itemDic[item2.Id] = item2;
        itemDic[item3.Id] = item3;
        itemDic[item4.Id] = item4;

        var res = Resources.Load("backPack/dragItem");
        GameObject dragItemObj = Instantiate(res) as GameObject;
        dragItemUI = dragItemObj.GetComponent<DragItemUI>();
        dragItemUI.transform.SetParent(transform);
        dragItemUI.Hide();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            int index = Random.Range(0, itemDic.Count);
            AddItem(index);
        }

        if (isDrag)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform,Input.mousePosition,null,out pos);
            dragItemUI.UpdatePosition(pos);
        }
    }



}
