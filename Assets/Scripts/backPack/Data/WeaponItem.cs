using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
///  武器道具
/// </summary>
public class WeaponItem : BaseItem
{
    //武器威力
    public int Damage { get; private set; }

    public WeaponItem(int id, string description, int damage)
        : base(id,description)
    {
        this.Damage = damage;
    }
}
