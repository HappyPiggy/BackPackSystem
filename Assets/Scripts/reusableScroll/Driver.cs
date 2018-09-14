using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData
{
    public string  name;
}

public class Driver :MonoBehaviour {

    private BaseCell baseCell;
    private BaseCellManager cellManager;

    private List<WeaponData> weaponDataList;

    void Start () {

        cellManager = transform.Find("root").gameObject.AddComponent<BaseCellManager>();
        weaponDataList = new List<WeaponData>();

        var res = Resources.Load("reusableScroll/cell");
        baseCell = Instantiate(res as GameObject).GetComponent<BaseCell>();

        cellManager.Init(baseCell);
        cellManager.SetParams(15,3,new Vector2 (0,0));

        weaponDataList.Clear();
        //模拟配置中读取的数据
        for (int i = 0; i < 100; i++)
        {
            var data = new WeaponData
            {
                name = "Name:" + i
            };
            weaponDataList.Add(data);
        }
        cellManager.SetData(weaponDataList);
    }

}
