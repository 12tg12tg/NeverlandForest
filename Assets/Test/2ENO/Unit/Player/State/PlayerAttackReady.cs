using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 공격 전 상태
public class PlayerAttackReady : State<CharacterBattleState>
{
    PlayerStats playerStat;
    Animator playerAnimation;

    public void SetPlayerStat(PlayerStats playerStat)
    {
        this.playerStat = playerStat;
    }
    public void SetPlayerAnimation(Animator playerAnimation)
    {
        this.playerAnimation = playerAnimation;
    }
    public override void Init()
    {
        // 공격 준비상태, 스킬슬롯 UI가 활성화되고 아이템 사용가능
    }
    public override void Release()
    {
        // 공격 이 종료된 시점 해야할 것들
    }
    public override void Update()
    {
        // 공격 하는순간을 기다렸다가 공격하면 몇초간 공격 애니메이션 실행,
        // 공격애니메이션 종료시 공격이 종료됬다는 것을 알려줌
    }





    public override void FixedUpdate()
    {
    }
    public override void LateUpdate()
    {
    }
}
