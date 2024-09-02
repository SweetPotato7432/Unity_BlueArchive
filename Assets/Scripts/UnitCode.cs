using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitCode
{
    // 아군 코드
    None,
    Azusa,
    Kasumi, 
    Hoshino,
    Haruna,
    Serina,
    Yoshimi,

    // 적군 코드
    EnemyNone=100,
    SukebanSmg,
    SukebanAr
}

// 현재 상태
public enum States
{
    None,
    Idle,
    Move,
    Cover,
    Attack,
    Reload,
    Dead
};

public enum AttackTypeCode
{
    None,
    Normal,
    Explosive,
    Piercing,
    Mystic,
    Sonic
}

public enum DefendTypeCode
{
    None,
    Normal,
    Light,
    Heavy,
    Special,
    Elastic
}

public enum WeaponCode
{
    None,
    Sr,
    Smg,
    Ar,
    Sg,
    Hg,
}
public enum BattleTypeCode
{
    None,
    Striker,
    Special
}
public enum DamageCode
{
    None,
    Resist,
    Normal,
    Effective,
    Weak
}

public enum GameSpeed
{
    None,
    Normal,
    Fast,
    Fastest
}