using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    // 유닛 구별을 위한 코드
    public UnitCode UnitCode { get; }
    // 유닛이 사용하는 무기 코드
    public WeaponCode WeaponCode { get; set; }
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
    public int AccuracyRate {  get; set; }
    // 회피율
    public int Dodge {  get; set; }
    // 치명타 확률
    public int CriticalRate {  get; set; }
    // 치명타 데미지 퍼센트
    public float CriticalDamage {  get; set; }
    // 사정거리
    public int Range {  get; set; }



}
