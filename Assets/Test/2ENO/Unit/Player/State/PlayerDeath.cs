using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 죽음 상태
public class PlayerDeath : State<CharacterBattleState>
{
    PlayerBattleController playerUnit;
    PlayerStats playerStat;
    Animator playerAnimation;

    public PlayerDeath(PlayerBattleController unit, Animator playerAnimation)
    {
        this.playerUnit = unit;
        this.playerAnimation = playerAnimation;
    }

    public override void Init()
    {
        // 캐릭터 애니메이션을 쓰러진 것으로 바꾸고
        // 배틀시스템 등에 죽었다는 메시지 보내기
    }
    public override void Release()
    {
        // 야영지에서 다시 부활시킬때 동작들?
    }
    public override void Update()
    {
    }



    public override void FixedUpdate()
    {
    }
    public override void LateUpdate()
    {
    }
}
