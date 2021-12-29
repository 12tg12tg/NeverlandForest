using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 공격이후 대기상태
// 몬스터 턴으로 넘어갔을 경우에도 이 상태 or 죽음상태
public class PlayerIdle : State<CharacterBattleState>, IAttackable
{
    PlayerStats playerStat;
    Animator playerAnimation;
    public void OnAttacked(UnitBase attacker)
    {
        // 피격당했을 시 피격 애니메이션 실행
    }
    public override void Init()
    {
        // 공격을 마쳣을때 이 상태가 된다.
    }
    public override void Release()
    {
        // 턴을 넘겨받았을 때 공격준비로 상태변화 or 피격중에 죽었을 경우도 이 함수 실행
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
