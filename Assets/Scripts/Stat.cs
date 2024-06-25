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
    public float MaxHp { get; set; }
    // 현재 체력
    public float CurHp { get; set; }
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
    // 공격속도
    public float AttackCoolTime { get; set; }
    // 최대 탄약 수
    public int MaxMag { get; set; }
    // 현재 장탄 수
    public int CurMag { get; set; }

    public Stat()
    {

    }
    public Stat(UnitCode unitCode, WeaponCode weaponCode,TypeCode typeCode, string name, float maxHp, int atk, int def, int accuracyRate,
        int dodge, int criticalRate, float criticalDamage, int range,int maxMag, bool isCoverAvailable)
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
        MaxMag = maxMag;
        CurMag = maxMag;
        IsCoverAvailable = isCoverAvailable;

        switch(weaponCode)
        {
            case WeaponCode.Sr:
                AttackCoolTime = 2f;
                break;
            case WeaponCode.Sg:
                AttackCoolTime = 1f;
                break;
            case WeaponCode.Hg:
                AttackCoolTime = 1.2f;
                break;
            default:
                AttackCoolTime = 1f;
                break;
        }


    }

    //
    public Stat setUnitStat(UnitCode unitCode)
    {
        Stat stat = null;
        switch (unitCode)
        {
            //                  유닛코드, 무기타입,     , 포지션           ,이름,   체력 ,공격,방어,명중,회피,치명,치뎀,사거리,탄,엄폐
            case UnitCode.Azusa:
                stat = new Stat(unitCode, WeaponCode.Ar, TypeCode.Striker, "아즈사", 2496f, 231, 19, 706, 792, 201, 2.0f, 650, 30, true);
                break;
            case UnitCode.Kasumi:
                stat = new Stat(unitCode, WeaponCode.Hg, TypeCode.Striker, "카스미", 2412f, 254, 19, 104, 1053, 208, 2.0f, 550, 8, true);
                break;
            case UnitCode.Hoshino:
                stat = new Stat(unitCode, WeaponCode.Sg, TypeCode.Striker, "호시노", 3275f, 213, 175, 615, 246, 205, 2.0f, 350, 8, false);
                break;
            case UnitCode.Haruna:
                stat = new Stat(unitCode, WeaponCode.Sr, TypeCode.Striker, "하루나", 2451f, 457, 19, 924, 205, 205, 2.0f,750, 5, true);
                break;
            case UnitCode.Serina:
                stat = new Stat(unitCode, WeaponCode.Ar, TypeCode.Special, "세리나", 2482f, 167, 24, 704, 805, 201, 2.0f, 1000, 30, false);
                break;
            case UnitCode.Yoshimi:
                stat = new Stat(unitCode, WeaponCode.Ar, TypeCode.Special, "요시미", 2232f, 268, 19, 705, 806, 201, 2.0f,1000, 30, false);
                break;
        }

        return stat;
    }
}


