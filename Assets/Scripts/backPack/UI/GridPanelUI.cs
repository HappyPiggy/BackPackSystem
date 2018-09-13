using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPanelUI : MonoBehaviour {

    private List<Transform> gridList;

	void Start () {

        InitGridList();
    }
	
	public Transform GetUsableGrid()
    {
        if (gridList == null)
        {
            InitGridList();
        }

        for (int i = 0; i < gridList.Count; i++)
        {
            if (gridList[i].childCount == 0)
                return gridList[i];
        }

        return null;
    }

    private void InitGridList()
    {
        gridList = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            gridList.Add(transform.GetChild(i));
        }
    }
}
