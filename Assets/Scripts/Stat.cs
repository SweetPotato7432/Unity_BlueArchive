using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    // 유닛 구별을 위한 코드
    public UnitCode UnitCode { get; }
    // 유닛이 사용하는 무기 코드
    public WeaponCode WeaponCode { get; set; }
    // 전투 타입코드
    public TypeCode TypeCode {  get; set; }
    // 유닛의 이름
    public string Name { get; set; }
    // 최대 체력
    public int MaxHp { get; set; }
    // 현재 체력
    public int CurHp { get; set; }
    // 공격력
    public int Atk { get; set; }
    // 방어력
    public int Def { get; set; }
    // 명중률
    public int AccuracyRate { get; set; }
    // 회피율
    public int Dodge { get; set; }
    // 치명타 확률
    public int CriticalRate { get; set; }
    // 치명타 데미지 퍼센트
    public float CriticalDamage { get; set; }
    // 사정거리
    public int Range { get; set; }
    // 엄폐 가능 여부
    public bool IsCoverAvailable { get; set; }

    public Stat()
    {

    }
    public Stat(UnitCode unitCode, WeaponCode weaponCode,TypeCode typeCode, string name, int maxHp, int atk, int def, int accuracyRate,
        int dodge, int criticalRate, float criticalDamage, int range, bool isCoverAvailable)
    {
        UnitCode = unitCode;
        WeaponCode = weaponCode;
        TypeCode = typeCode;
        Name = name;
        MaxHp = maxHp;
        CurHp = maxHp;
        Atk = atk;
        Def = def;
        AccuracyRate = accuracyRate;
        Dodge = dodge;
        CriticalRate = criticalRate;
        CriticalDamage = criticalDamage;
        Range = range;
        IsCoverAvailable = isCoverAvailable;
    }

    //
    public Stat setUnitStat(UnitCode unitCode)
    {
        Stat stat = null;
        switch (unitCode)
        {
            case UnitCode.Aru:
                stat = new Stat(unitCode, WeaponCode.Sr,TypeCode.Striker, "Aru", 2505, 451, 19, 905, 201, 201, 2.0f, 750, true);
                break;
            case UnitCode.Kasumi:
                stat = new Stat(unitCode, WeaponCode.Hg, TypeCode.Striker, "Kasumi", 2412, 254, 19, 104, 1053, 208, 2.0f, 550, true);
                break;
            case UnitCode.Hoshino:
                stat = new Stat(unitCode, WeaponCode.Sg, TypeCode.Striker, "Hoshino", 3275, 213, 175, 615, 246, 205, 2.0f, 350, false);
                break;
            case UnitCode.Haruna:
                stat = new Stat(unitCode, WeaponCode.Sr, TypeCode.Striker, "Haruna", 2451, 457, 19, 924, 205, 205, 2.0f, 750, true);
                break;
            case UnitCode.Serina:
                stat = new Stat(unitCode, WeaponCode.Ar, TypeCode.Special, "Serina", 2482, 167, 24, 704, 805, 201, 2.0f, 1000, false);
                break;
            case UnitCode.Yoshimi:
                stat = new Stat(unitCode, WeaponCode.Ar, TypeCode.Special, "Yoshimi", 2232, 268, 19, 705, 806, 201, 2.0f, 1000, false);
                break;
        }

        return stat;
    }
}


