using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// 技能道具
/// </summary>
public class SkillItem:BaseItem
{
    //技能范围
    public int Range { get; private set; }

    public SkillItem(int id,  string description, int range)
         : base(id, description)
    {
        this.Range = range;
    }
}