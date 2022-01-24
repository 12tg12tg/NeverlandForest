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

    public void ArrowCreate() // ��ɲ��� ���� �ڷ� ���� �� ȭ���� ���� �� ó�� �ϱ� ���� �ʿ��� ��
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