using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCell : BaseCell {

    private Text text;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
    }



    public override void UpdateInfo(int index, object data)
    {
        base.UpdateInfo(index, data);
        WeaponData weaponData = data as WeaponData;
        text.text = weaponData.name;
    }

    public override float GetCellWidth()
    {
        return 150;
    }

    public override float GetCellHeight()
    {
        return 100;
    }
}
