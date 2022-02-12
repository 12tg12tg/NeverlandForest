using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFunc : MonoBehaviour
{
    public PlayerAction actionState;
    public HuntingManager huntingManager;
    public GatheringSystem gathringSystem;
    public GameObject arrow;
    public GameObject axe;
    public GameObject shovel;

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
        huntingManager.Shooting();
    }

    public void GatherIngPickUpEnd()
    {
        gathringSystem.GatheringEnd();
        axe.SetActive(false);
        shovel.SetActive(false);
    }

    public void GatheringUseAxe()
    {
        axe.SetActive(true);
    }

    public void GatheringUseShovel()
    {
        Debug.Log("�� �����");
        shovel.SetActive(true);
    }
}