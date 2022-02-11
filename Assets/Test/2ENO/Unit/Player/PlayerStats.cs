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
        if(ConsumeManager.GetDamage(damage))
        {
            // ���� ��
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
            // �������
            controller.PlayHitAnimation();
        }

        Debug.Log($"{controller.playerType}�� {attackUnit}���� {damage}�� ���ظ� �޴�.");
        Debug.Log($"{Vars.UserData.uData.Hp + damage} -> {Vars.UserData.uData.Hp}");
    }
}
