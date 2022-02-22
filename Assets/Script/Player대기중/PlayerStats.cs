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
        if(ConsumeManager.GetDamage(damage))
        {
            // 전투 끝
            manager.boy.PlayDeadAnimation();
            manager.girl.PlayDeadAnimation();
            if(manager.isTutorial)
            {
                manager.tutorial.isLose = true;
            }
            else
            {
                manager.Lose();
            }
        }
        else
        {
            // 살아있음
            controller.PlayHitAnimation();
        }

        Debug.Log($"{controller.playerType}가 {attackUnit}에게 {damage}의 피해를 받다.");
        Debug.Log($"{Vars.UserData.uData.Hp + damage} -> {Vars.UserData.uData.Hp}");
    }
}
