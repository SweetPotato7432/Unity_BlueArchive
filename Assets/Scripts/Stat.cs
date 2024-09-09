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
    public BattleTypeCode BattleTypeCode {  get; set; }
    // 데미지 타입 코드
    public AttackTypeCode DamageTypeCode { get; set; }
    // 방어 타입 코드
    public DefendTypeCode DefendTypeCode { get; set; }
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
    public float Range { get; set; }
    // 엄폐 가능 여부
    public bool IsCoverAvailable { get; set; }
    // 공격속도
    public float AttackCoolTime { get; set; }
    // 최대 탄약 수
    public int MaxMag { get; set; }
    // 현재 장탄 수
    public int CurMag { get; set; }
    // 재장전 시간
    public float ReloadTime { get; set; }
    // 엄폐 성공 확률(30%)
    public float CoverRate { get; set; }
    // 엄폐 중 체크
    public bool IsCover { get;set; }

    // 기본 생성자
    public Stat()
    {

    }
    // 초기화 생성자
    public Stat(UnitCode unitCode, WeaponCode weaponCode,BattleTypeCode battleTypeCode,AttackTypeCode damageTypeCode, DefendTypeCode defendTypeCode, string name, float maxHp, int atk, int def, int accuracyRate,
        int dodge, int criticalRate, float criticalDamage, float range,int maxMag, bool isCoverAvailable)
    {
        UnitCode = unitCode;
        WeaponCode = weaponCode;
        BattleTypeCode = battleTypeCode;
        DamageTypeCode = damageTypeCode;
        DefendTypeCode = defendTypeCode;
        Name = name;
        MaxHp = maxHp;
        CurHp = maxHp;
        Atk = atk;
        Def = def;
        AccuracyRate = accuracyRate;
        Dodge = dodge;
        CriticalRate = criticalRate;
        CriticalDamage = criticalDamage;
        Range = range * 0.035f;
        MaxMag = maxMag;
        CurMag = maxMag;
        IsCoverAvailable = isCoverAvailable;
        CoverRate = 0.3f;
        IsCover = false;



        SetWeaponProperties(WeaponCode);


    }

    // 무기 속성 설정
    private void SetWeaponProperties(WeaponCode weaponCode)
    {
        switch (weaponCode)
        {
            case WeaponCode.Sr:
                AttackCoolTime = 3f;
                ReloadTime = 2.3f;
                break;
            case WeaponCode.Sg:
                AttackCoolTime = 1f;
                ReloadTime = 2f;
                break;
            case WeaponCode.Hg:
                AttackCoolTime = 1.2f;
                ReloadTime = 1.8f;
                break;
            case WeaponCode.Ar:
                AttackCoolTime = 2f;
                ReloadTime = 2f;
                break;
            case WeaponCode.Smg:
                AttackCoolTime = 1.5f;
                ReloadTime = 2f;
                break;
            default:
                AttackCoolTime = 1f;
                ReloadTime = 2f;
                break;
        }
    }
    //
    public static Stat setUnitStat(UnitCode unitCode)
    {
        return unitCode switch
        {   
            //                         유닛코드, 무기타입,     , 포지션               ,  공격타입               , 방어타입             , 이름,   체력 ,공격,방어,명중,회피,치명,치뎀,사거리,탄,엄폐
            UnitCode.Azusa => new Stat(unitCode, WeaponCode.Ar, BattleTypeCode.Striker, AttackTypeCode.Explosive, DefendTypeCode.Heavy, "아즈사", 2496f, 231, 19, 706, 792, 201, 2.0f, 650f, 10, true),
            UnitCode.Kasumi => new Stat(unitCode, WeaponCode.Hg, BattleTypeCode.Striker, AttackTypeCode.Sonic, DefendTypeCode.Heavy, "카스미", 2412f, 254, 19, 104, 1053, 208, 2.0f, 550f, 8, true),
            UnitCode.Hoshino => new Stat(unitCode, WeaponCode.Sg, BattleTypeCode.Striker, AttackTypeCode.Piercing, DefendTypeCode.Heavy, "호시노", 3275f, 213, 175, 615, 246, 205, 2.0f, 350f, 8, false),
            UnitCode.Haruna => new Stat(unitCode, WeaponCode.Sr, BattleTypeCode.Striker, AttackTypeCode.Mystic, DefendTypeCode.Heavy, "하루나", 2451f, 457, 19, 924, 205, 205, 2.0f, 750f, 5, true),
            UnitCode.Serina => new Stat(unitCode, WeaponCode.Ar, BattleTypeCode.Special, AttackTypeCode.Mystic, DefendTypeCode.Light, "세리나", 2482f, 167, 24, 704, 805, 201, 2.0f, 1000f, 10, false),
            UnitCode.Yoshimi => new Stat(unitCode, WeaponCode.Ar, BattleTypeCode.Special, AttackTypeCode.Explosive, DefendTypeCode.Heavy, "요시미", 2232f, 268, 19, 705, 806, 201, 2.0f, 1000f, 10, false),
            UnitCode.SukebanSmg => new Stat(unitCode, WeaponCode.Smg, BattleTypeCode.Striker, AttackTypeCode.Normal, DefendTypeCode.Light, "불량배(SMG)", 640f, 18, 30, 98, 1416, 196, 2.0f, 350f, 10, false),
            UnitCode.SukebanAr => new Stat(unitCode, WeaponCode.Ar, BattleTypeCode.Striker, AttackTypeCode.Normal, DefendTypeCode.Heavy, "불량배(AR)", 500f, 23, 19, 706, 792, 201, 2.0f, 650f, 10, true),
            _ => null,

        };
    }
}


