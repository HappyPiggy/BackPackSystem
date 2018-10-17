using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 按页面滚动
/// 挂在scrollview的同级gameobject上
/// </summary>
public class PageScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{

    public bool isToggle = true;
    public List<float> pageArray;
    private List<GameObject> pageList;
    private List<Toggle> toggleArray;
    private ToggleGroup toggleGroup;

    private ScrollRect rect;
    private GameObject pageRoot;
    private GameObject toggleRoot;


    private bool isDrag=false;
    private int pageCount=1;
    private float smooth=5;

    private float targethorizontal;


    private void Start()
    {
        Init();

        GeneratePage();
    }

    /// <summary>
    /// 模拟外部调用
    /// </summary>
    private void GeneratePage()
    {
        List<GameObject> pages = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            GameObject pageItem = Instantiate(Resources.Load("pageScroll/page")) as GameObject;
            pageItem.name = pageItem.name + i;
            pages.Add(pageItem);
        }

        SetPagesData(pages);
    }

    private void GenerateToggle()
    {
        toggleArray = new List<Toggle>();
        toggleGroup = pageRoot.AddComponent<ToggleGroup>();

        toggleRoot = transform.Find("ToggleRoot").gameObject;
        if (toggleRoot == null) Debug.LogError("不存在ToggleRoot节点");

        for (int i = 0; i <pageCount; i++)
        {
            GameObject toggleObj = Instantiate(Resources.Load("pageScroll/Toggle")) as GameObject;
            toggleObj.transform.SetParent(toggleRoot.transform);

            Toggle toggle = toggleObj.GetComponent<Toggle>();
            toggle.group = toggleGroup;

            int idx = i;
            toggle.onValueChanged.AddListener((bool value) =>
            {
                if (value)
                {
                    targethorizontal= pageArray[idx];
                }
            }
            );
            toggleArray.Add(toggle);
        }

        foreach (var item in toggleArray)
        {
            item.isOn = false;
        }

        if (toggleArray.Count > 0)
        {
            toggleArray[0].isOn = true;
        }
    }


    private void Init()
    {
        rect = transform.GetComponent<ScrollRect>();
        rect.horizontalNormalizedPosition = 0;

        pageRoot = transform.Find("Viewport/Content").gameObject;
        if (pageRoot == null) Debug.LogError("不存在content节点");

        pageArray = new List<float>();
        pageList = new List<GameObject>();

        CleanDirty();

    }

    /// <summary>
    /// 外部调用传入要生成页面的数据
    /// 现内部模拟调用
    /// </summary>
    public void SetPagesData(List<GameObject> pages)
    {
        if (pageRoot != null)
        {
            pageCount = pages.Count;
            for (int i = 0; i < pageCount; i++)
            {
                pages[i].transform.SetParent(pageRoot.transform);
                pages[i].transform.localScale = new Vector3(1, 1, 1);
                pages[i].transform.localRotation = new Quaternion();
                pages[i].transform.localPosition = Vector3.one;
                pageList.Add(pages[i]);
            }

            int num = pageCount - 1;
            if (num == 0) num = 1;
            for (int i = 0; i < pageCount; i++)
            {
                pageArray.Add((float)i / num);
            }

            if (isToggle)
                GenerateToggle();
        }
    }

    private void Update()
    {
        //平滑
        if (!isDrag)
        {
            rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, Time.deltaTime * smooth);
        }
    }





    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;


        float posX = rect.horizontalNormalizedPosition;
        int index = 0;
        float offset = Mathf.Abs(pageArray[index] - posX);
        for (int i = 1; i < pageArray.Count; i++)
        {
            float temp = Mathf.Abs(pageArray[i] - posX);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }
        targethorizontal = pageArray[index];

        if (isToggle && pageCount > 1 && toggleArray.Count > 0)
            toggleArray[index].isOn = true;
    }


    private void CleanDirty()
    {
        for (int i = 0; i < pageList.Count; i++)
        {
           Destroy(pageList[i]);
        }
        pageList.Clear();
    }


}
