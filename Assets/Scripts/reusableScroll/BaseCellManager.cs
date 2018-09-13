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
    public Vector3 startPos;

    private List<object> cellDataList; //读取所有cell的数据 随着拖拉动态更新

    //todo 用链表更合适
    private List<BaseCell> cellObjList;//存储当前可视的cell的obj 

    private BaseCell originCellObj;
    private float cellHeight;
    private float cellWidth;

    private int cellIndex;
    private float viewHeight; //视区高度

    private ScrollRect scrollRect;
    private Transform cellRoot;


    private void Awake()
    {
        cellDataList = new List<object>();
        cellObjList = new List<BaseCell>();
        scrollRect = GetComponentInParent<ScrollRect>();
        cellRoot = transform;
    }

    /// <summary>
    /// 设置列表参数
    /// </summary>
    /// <param name="cellCount">可视cell个数</param>
    /// <param name="rowCount">列数</param>
    /// <param name="startPos">第一个cell位置</param>
    public void SetParams(int cellCount,int rowCount,Vector3 startPos)
    {
        this.cellCount = cellCount;
        this.rowCount = rowCount;
        this.startPos = startPos;
    }

    /// <summary>
    /// 用该manager时都需要初始化
    /// </summary>
    /// <param name="baseCell">需要创建的cell的obj</param>
    public void Init(BaseCell baseCell)
    {
        originCellObj = baseCell;
        cellHeight = originCellObj.GetCellHeight();
        cellWidth = originCellObj.GetCellWidth();

        Vector3[] corners = new Vector3[4];
        scrollRect.transform.GetComponent<RectTransform>().GetLocalCorners(corners);
        viewHeight = corners[1].y + cellHeight;

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
            for (int i = 0; i < cellObjList.Count; i++)
            {
                cellObjList[i].gameObject.SetActive(false);
            }
        }

        cellDataList = Convert(dataList);
        SetContentSize();
        cellIndex = 0;
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
                baseCell.UpdateInfo(index, cellDataList[index]);
            }
            else
            {
                baseCell.gameObject.SetActive(false);
            }
            cellObjList.Add(baseCell);
        }else if(cellIndex<cellObjList.Count && cellIndex < cellDataList.Count)//显示视区cell
        {
            cellObjList[cellIndex].gameObject.SetActive(true);
            cellObjList[cellIndex].transform.localPosition = GetCellPosition(cellIndex);
            cellObjList[cellIndex].UpdateInfo(cellIndex,cellDataList[cellIndex]);
            cellIndex++;
        }else if (cellCount < cellDataList.Count) //超出视区则移位置reuse
        {

            var scrollPos = transform.localPosition;
            if(cellObjList[0].transform.localPosition.y+scrollPos.y > viewHeight) 
            {
                //上侧出视区的obj移到下侧
                //当前视区的最后一个obj已经是最末端数据，则到达低端，不需要再将上侧obj转移到下侧
                if (cellObjList[cellObjList.Count-1].index <cellDataList.Count-1)
                {
                    int index = cellObjList[cellObjList.Count - 1].index + 1;
                    var pos = GetCellPosition(index);

                    cellObjList[0].UpdateInfo(index,cellDataList[index]);
                    cellObjList[0].transform.localPosition = pos;

                    var cell = cellObjList[0];
                    cellObjList.RemoveAt(0);
                    cellObjList.Add(cell);
                }

            }
            else  if(cellObjList[cellObjList.Count-1].transform.localPosition.y + scrollPos.y < -viewHeight)
            {
                //下侧出视区的obj转移到上侧
                //当前视区的第一个obj已经是第一个数据，则到达顶端，不需要再将下侧obj转移到上侧
                if (cellObjList[0].index > 0)
                {
                    int index = cellObjList[0].index - 1;
                    var pos = GetCellPosition(index);

                    cellObjList[cellObjList.Count - 1].UpdateInfo(index,cellDataList[index]);
                    cellObjList[cellObjList.Count - 1].transform.localPosition = pos;

                    var cell = cellObjList[cellObjList.Count - 1];
                    cellObjList.RemoveAt(cellObjList.Count-1);
                    cellObjList.Insert(0,cell);
                }
            }

        }
    }


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
