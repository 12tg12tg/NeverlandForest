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

    // 전투 시스템에서 현재 선택한 스킬 정보
    public DataPlayerSkill selectSkill;

    public void OnAttacked(BattleCommand attacker)
    {
        var attackUnit = attacker as MonsterCommand;
        var damage = attackUnit.attacker.Atk;
        Hp -= damage;
        Debug.Log($"{controller.playerType}가 {attackUnit}에게 {damage}의 피해를 받다.");
        Debug.Log($"{Hp + damage} -> {Hp}");
    }
}
