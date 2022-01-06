using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : UnitBase, IAttackable
{
    // Instance
    public BattleManager manager;
    public PlayerBattleController controller;

    // Vars
    private bool isBind;
    // ���߿� flag enum���� ����
    private bool isBuff;
    private bool isDeBuff;
    private int def;
    // �ӽ�: ���� ������ ����Ʈ ������ �ֱ�
    //private List<DataItem> equipItemList = new List<DataItem>();

    // Property
    public int Def { set => def = value; get => def; }
    public bool IsBind { set => isBind = true; get => isBind; }
    public bool IsBuff { set => isBuff = value; get => isBuff; }
    public bool IsDeBuff { set => isDeBuff = value; get => isDeBuff; }

    private List<DataPlayerSkill> skillList = new List<DataPlayerSkill>();
    public List<DataPlayerSkill> SkillList
    {
        get => skillList;
    }

    // ���� �ý��ۿ��� ���� ������ ��ų ����
    public DataPlayerSkill selectSkill;

    public void OnAttacked(BattleCommand attacker)
    {
        var attackUnit = attacker as MonsterCommand;
        var damage = attackUnit.attacker.Atk;
        Hp -= damage;
        Debug.Log($"{controller.playerType}�� {attackUnit}���� {damage}�� ���ظ� �޴�.");
        Debug.Log($"{Hp + damage} -> {Hp}");
    }
}
