using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFunc : MonoBehaviour
{
    public PlayerAction actionState;
    public HuntingManager huntingSystem;
    public GatheringSystem gathringSystem;
    public GameObject arrow;

    public void Hit()
    {
        actionState.isAttackMotionEnd = true;
        Debug.Log("Animation Hit Func");
    }

    public void ArrowCreate() // 사냥꾼의 손이 뒤로 갔을 때 화살을 집는 것 처럼 하기 위해 필요한 것
    {
        arrow.SetActive(true);
    }

    public void Shoot()
    {
        arrow.SetActive(false);
        huntingSystem.Shooting();
    }

    public void GatherIngPickUpEnd()
    {
        gathringSystem.GatheringEnd();
    }
}