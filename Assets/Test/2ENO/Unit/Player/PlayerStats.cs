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
    // 나중에 flag enum으로 구현
    private bool isBuff;
    private bool isDeBuff;
    private int def;
    // 임시: 장착 아이템 리스트 가지고 있기
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
