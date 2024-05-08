using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    public int HP;
    public int MP;
    public float SP;
    public int XP;
    public int Damage;
    public int Defense;

    public static PlayerStat operator +(PlayerStat a, PlayerStat b)
    {
        return new PlayerStat
        {
            HP = a.HP + b.HP,
            MP = a.MP + b.MP,
            SP = a.SP + b.SP,
            XP = a.XP + b.XP,
            Damage = a.Damage + b.Damage,
            Defense = a.Defense + b.Defense,
        };
    }

    public static PlayerStat operator -(PlayerStat a, PlayerStat b)
    {
        return new PlayerStat
        {
            HP = a.HP - b.HP,
            MP = a.MP - b.MP,
            SP = a.SP - b.SP,
            XP = a.XP - b.XP,
            Damage = a.Damage - b.Damage,
            Defense = a.Defense - b.Defense,
        };
    }
}
