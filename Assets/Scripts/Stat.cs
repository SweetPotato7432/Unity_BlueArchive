using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    // ���� ������ ���� �ڵ�
    public UnitCode UnitCode { get; }
    // ������ ����ϴ� ���� �ڵ�
    public WeaponCode WeaponCode { get; set; }
    // ������ �̸�
    public string Name { get; set; }
    // �ִ� ü��
    public int MaxHp { get; set; }
    // ���� ü��
    public int CurHp { get; set; }
    // ���ݷ�
    public int Atk { get; set; }
    // ����
    public int Def { get; set; }
    // ���߷�
    public int AccuracyRate {  get; set; }
    // ȸ����
    public int Dodge {  get; set; }
    // ġ��Ÿ Ȯ��
    public int CriticalRate {  get; set; }
    // ġ��Ÿ ������ �ۼ�Ʈ
    public float CriticalDamage {  get; set; }
    // �����Ÿ�
    public int Range {  get; set; }



}
