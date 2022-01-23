using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : UnitBase, IAttackable
{
    // Instance
    public BattleManager manager;
    public PlayerBattleController controller;

    // Property
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
