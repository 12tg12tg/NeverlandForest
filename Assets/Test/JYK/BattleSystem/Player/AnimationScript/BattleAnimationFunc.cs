using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAnimationFunc : MonoBehaviour
{
    public PlayerBattleController controller;
    public PlayerAction actionState;
    public GameObject arrow;
    public GameObject hitArrow;

    public Vector3 shootStartPos;
    public float angle;

    private const float speed = 30f;
    private const float gravity = 9.8f;
    private const float maxDistance = 30f;
    public void Hit()
    {
        actionState.isAttackMotionEnd = true;
    }

    public void ArrowCreate() // ��ɲ��� ���� �ڷ� ���� �� ȭ���� ���� �� ó�� �ϱ� ���� �ʿ��� ��
    {
        arrow.SetActive(true);
    }

    public void Shoot()
    {
        shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrow(dest);
    }

    public void ShootArrow(Vector3 pos)
    {
        var shootArrow = ProjectilePool.Instance.GetObject(ProjectileTag.HunterArrow);
        shootArrow.transform.position = shootStartPos;
        var arrow = shootArrow.GetComponent<Arrow>();
        StartCoroutine(arrow.Shoot(pos));
    }


}