using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    // ���� ������ ���� �ڵ�
    public UnitCode UnitCode { get; }
    // ������ ����ϴ� ���� �ڵ�
    public WeaponCode WeaponCode { get; set; }
    // ���� Ÿ���ڵ�
    public BattleTypeCode BattleTypeCode {  get; set; }
    // ������ Ÿ�� �ڵ�
    public AttackTypeCode DamageTypeCode { get; set; }
    // ��� Ÿ�� �ڵ�
    public DefendTypeCode DefendTypeCode { get; set; }
    // ������ �̸�
    public string Name { get; set; }
    // �ִ� ü��
    public float MaxHp { get; set; }
    // ���� ü��
    public float CurHp { get; set; }
    // ���ݷ�
    public int Atk { get; set; }
    // ����
    public int Def { get; set; }
    // ���߷�
    public int AccuracyRate { get; set; }
    // ȸ����
    public int Dodge { get; set; }
    // ġ��Ÿ Ȯ��
    public int CriticalRate { get; set; }
    // ġ��Ÿ ������ �ۼ�Ʈ
    public float CriticalDamage { get; set; }
    // �����Ÿ�
    public float Range { get; set; }
    // ���� ���� ����
    public bool IsCoverAvailable { get; set; }
    // ���ݼӵ�
    public float AttackCoolTime { get; set; }
    // �ִ� ź�� ��
    public int MaxMag { get; set; }
    // ���� ��ź ��
    public int CurMag { get; set; }
    // ������ �ð�
    public float ReloadTime { get; set; }
    // ���� ���� Ȯ��(30%)
    public float CoverRate { get; set; }
    // ���� �� üũ
    public bool IsCover { get;set; }

    // �⺻ ������
    public Stat()
    {

    }
    // �ʱ�ȭ ������
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

    // ���� �Ӽ� ����
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
            //                         �����ڵ�, ����Ÿ��,     , ������               ,  ����Ÿ��               , ���Ÿ��             , �̸�,   ü�� ,����,���,����,ȸ��,ġ��,ġ��,��Ÿ�,ź,����
            UnitCode.Azusa => new Stat(unitCode, WeaponCode.Ar, BattleTypeCode.Striker, AttackTypeCode.Explosive, DefendTypeCode.Heavy, "�����", 2496f, 231, 19, 706, 792, 201, 2.0f, 650f, 10, true),
            UnitCode.Kasumi => new Stat(unitCode, WeaponCode.Hg, BattleTypeCode.Striker, AttackTypeCode.Sonic, DefendTypeCode.Heavy, "ī����", 2412f, 254, 19, 104, 1053, 208, 2.0f, 550f, 8, true),
            UnitCode.Hoshino => new Stat(unitCode, WeaponCode.Sg, BattleTypeCode.Striker, AttackTypeCode.Piercing, DefendTypeCode.Heavy, "ȣ�ó�", 3275f, 213, 175, 615, 246, 205, 2.0f, 350f, 8, false),
            UnitCode.Haruna => new Stat(unitCode, WeaponCode.Sr, BattleTypeCode.Striker, AttackTypeCode.Mystic, DefendTypeCode.Heavy, "�Ϸ糪", 2451f, 457, 19, 924, 205, 205, 2.0f, 750f, 5, true),
            UnitCode.Serina => new Stat(unitCode, WeaponCode.Ar, BattleTypeCode.Special, AttackTypeCode.Mystic, DefendTypeCode.Light, "������", 2482f, 167, 24, 704, 805, 201, 2.0f, 1000f, 10, false),
            UnitCode.Yoshimi => new Stat(unitCode, WeaponCode.Ar, BattleTypeCode.Special, AttackTypeCode.Explosive, DefendTypeCode.Heavy, "��ù�", 2232f, 268, 19, 705, 806, 201, 2.0f, 1000f, 10, false),
            UnitCode.SukebanSmg => new Stat(unitCode, WeaponCode.Smg, BattleTypeCode.Striker, AttackTypeCode.Normal, DefendTypeCode.Light, "�ҷ���(SMG)", 640f, 18, 30, 98, 1416, 196, 2.0f, 350f, 10, false),
            UnitCode.SukebanAr => new Stat(unitCode, WeaponCode.Ar, BattleTypeCode.Striker, AttackTypeCode.Normal, DefendTypeCode.Heavy, "�ҷ���(AR)", 500f, 23, 19, 706, 792, 201, 2.0f, 650f, 10, true),
            _ => null,

        };
    }
}


