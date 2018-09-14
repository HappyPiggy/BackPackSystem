using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 负责创建和更新cell
/// </summary>
public class BaseCellManager:MonoBehaviour
{
    public int cellCount;
    public int rowCount;
    public Vector2 spacing;
    public Vector3 startPos;

    private List<object> cellDataList; //读取所有cell的数据 随着拖拉动态更新

    private LinkedList<BaseCell> cellObjList;//存储当前可视的cell的obj 

    private BaseCell originCellObj;
    private float cellHeight;
    private float cellWidth;

    private float viewHeight; //视区高度

    private ScrollRect scrollRect;
    private Transform cellRoot;


    private void Awake()
    {
        cellDataList = new List<object>();
        cellObjList = new LinkedList<BaseCell>();
        scrollRect = GetComponentInParent<ScrollRect>();
        cellRoot = transform;
        SetAnchors();
    }

    /// <summary>
    /// 设置列表参数
    /// </summary>
    /// <param name="cellCount">可视cell个数</param>
    /// <param name="rowCount">列数</param>
    /// <param name="spacing">x y 间距</param>
    public void SetParams(int cellCount,int rowCount,Vector2 spacing)
    {
        this.cellCount = cellCount;
        this.rowCount = rowCount;
        this.spacing = spacing;

        cellHeight += spacing.y;
        cellWidth += spacing.x;
    }

    /// <summary>
    /// 用该manager时都需要初始化
    /// </summary>
    /// <param name="baseCell">需要创建的cell的obj</param>
    public void Init(BaseCell baseCell)
    {
        originCellObj = baseCell;
        cellHeight += originCellObj.GetCellHeight() + spacing.y;
        cellWidth += originCellObj.GetCellWidth() + spacing.x;

        Vector3[] corners = new Vector3[4];
        scrollRect.transform.GetComponent<RectTransform>().GetLocalCorners(corners);
        viewHeight = corners[1].y + cellHeight;

        startPos = new Vector3(cellWidth/2,-cellHeight/2);

        if (cellHeight == 0 || cellWidth == 0)
            Debug.LogError("cell宽或高不能为0");
    }

    /// <summary>
    /// 将数据传入
    /// 初始化后调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataList"></param>
    public void SetData<T>(List<T> dataList)
    {
        if (cellObjList != null)
        {
            foreach (var item in cellObjList)
            {
                item.gameObject.SetActive(false);
            }
        }

        cellDataList = Convert(dataList);
        SetContentSize();
    }


    private void Update()
    {
        CellUpdate();
    }


    /// <summary>
    /// 根据拖动的位置更新每个cell的info
    /// </summary>
    private void CellUpdate()
    {
        BaseCell baseCell;
        //第一次进入需要创建
        if(cellObjList.Count< cellCount)
        {
            int index = cellObjList.Count;
            baseCell = (Instantiate(originCellObj.gameObject) as GameObject).GetComponent<BaseCell>();
            baseCell.transform.parent = cellRoot;
            baseCell.transform.localScale = Vector3.one;
           // baseCell.SetAnchors(scrollRect.content.anchorMin, scrollRect.content.anchorMax);

            //不在视区的暂时不更info
            if (index < cellDataList.Count)
            {
                baseCell.gameObject.SetActive(true);
                baseCell.transform.localPosition = GetCellPosition(index);
                baseCell.UpdateInfo(index, cellDataList[index]);
            }
            else
            {
                baseCell.gameObject.SetActive(false);
            }
            cellObjList.AddLast(baseCell);
        }else if (cellCount < cellDataList.Count) //超出视区则移位置reuse
        {

            var scrollPos = transform.localPosition;
            if(cellObjList.First.Value.transform.localPosition.y+scrollPos.y > viewHeight) 
            {
                //上侧出视区的obj移到下侧
                //当前视区的最后一个obj已经是最末端数据，则到达低端，不需要再将上侧obj转移到下侧
                if (cellObjList.Last.Value.index <cellDataList.Count-1)
                {
                    int index = cellObjList.Last.Value.index + 1;
                    var pos = GetCellPosition(index);

                    //避免频繁更新 只在要进入视区时更新
                    if (pos.y + scrollPos.y > -viewHeight)
                    {
                        cellObjList.First.Value.UpdateInfo(index, cellDataList[index]);
                        cellObjList.First.Value.transform.localPosition = pos;

                        var cell = cellObjList.First.Value;
                        cellObjList.RemoveFirst();
                        cellObjList.AddLast(cell);
                    }
          
                }

            }
            else  if(cellObjList.Last.Value.transform.localPosition.y + scrollPos.y < -viewHeight)
            {
                //下侧出视区的obj转移到上侧
                //当前视区的第一个obj已经是第一个数据，则到达顶端，不需要再将下侧obj转移到上侧
                if (cellObjList.First.Value.index > 0)
                {
                    int index = cellObjList.First.Value.index - 1;
                    var pos = GetCellPosition(index);

                    if (pos.y + scrollPos.y < viewHeight)
                    {
                        cellObjList.Last.Value.UpdateInfo(index, cellDataList[index]);
                        cellObjList.Last.Value.transform.localPosition = pos;

                        var cell = cellObjList.Last.Value;
                        cellObjList.RemoveLast();
                        cellObjList.AddFirst(cell);
                    }

                }
            }

        }
    }

    /// <summary>
    /// 设定content的anchor
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void SetAnchors()
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.up;
        rectTransform.anchorMax = Vector2.one;

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.pivot = new Vector2(0, 1);
    }

    /// <summary>
    /// 动态设定scroll的content高度
    /// </summary>
    private void SetContentSize()
    {
        Vector2 sizeDelta = scrollRect.content.sizeDelta;
        float contentSize = 0;
        for (int i = 0; i <=cellDataList.Count/rowCount; i++)
        {
            contentSize += cellHeight;
        }
        sizeDelta.y = contentSize;
        scrollRect.content.sizeDelta = sizeDelta;
    }

    /// <summary>
    /// 根据index得到cell的localPosition
    /// </summary>
    /// <param name="index">cell的index</param>
    /// <returns></returns>
    private Vector3 GetCellPosition(int index)
    {
        var pos = startPos + Vector3.right * cellWidth * (index % rowCount) + Vector3.down * (cellHeight * (index / rowCount));
        return pos;
    }

    /// <summary>
    /// 泛型list转换为object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    private List<object> Convert<T>(List<T> list)
    {
        List<object> retList = new List<object>();
        foreach (var item in list)
        {
            retList.Add(item);
        }
        return retList;
    }

}
