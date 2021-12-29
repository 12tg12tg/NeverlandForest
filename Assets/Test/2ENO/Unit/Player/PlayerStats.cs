using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : UnitBase, IAttackable
{
    private bool isBind;
    // ���߿� flag enum���� ����
    private bool isBuff;
    private bool isDeBuff;

    private int def;
    public int Def
    {
        set => def = value;
        get => def;
    }

    public bool IsBind
    {
        set => isBind = true;
        get => isBind;
    }
    public bool IsBuff
    {
        set => isBuff = value;
        get => isBuff;
    }
    public bool IsDeBuff
    {
        set => isDeBuff = value;
        get => isDeBuff;
    }

    // �ӽ�: ���� ������ ����Ʈ ������ �ֱ�
    //private List<DataItem> equipItemList = new List<DataItem>();

    private List<DataPlayerSkill> skillList = new List<DataPlayerSkill>();
    public List<DataPlayerSkill> SkillList
    {
        get => skillList;
    }

    // ���� �ý��ۿ��� ���� ������ ��ų ����
    public DataPlayerSkill selectSkill;



    public void OnAttacked(UnitBase attacker)
    {
        var attackUnit = attacker as PlayerStats;

        Hp -= attackUnit.Atk;
        Debug.Log($"Unit:{attacker.gameObject} �� Unit:{gameObject} �� ����");
    }
}
