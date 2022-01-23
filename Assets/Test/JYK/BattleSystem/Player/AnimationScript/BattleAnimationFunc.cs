using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    // Attack 애니메이션
    public void Hit()
    {
        actionState.isAttackMotionEnd = true;
    }

    // Shoot 애니메이션
    public void ArrowCreate() // 사냥꾼의 손이 뒤로 갔을 때 화살을 집는 것 처럼 하기 위해 필요한 것
    {
        arrow.SetActive(true);
    }

    public void Shoot()
    {
        shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowCurve(dest);
    }

    public void ShootArrowCurve(Vector3 pos)
    {
        var shootArrow = ProjectilePool.Instance.GetObject(ProjectileTag.HunterArrow);
        shootArrow.transform.position = shootStartPos;
        var arrow = shootArrow.GetComponent<Arrow>();
        StartCoroutine(arrow.Shoot(pos));
    }

    public void ShootArrowLine(Vector3 pos, bool isFianlShot = false)
    {
        var shootArrow = ProjectilePool.Instance.GetObject(ProjectileTag.HunterArrow);
        shootArrow.transform.position = shootStartPos;

        var arrow = shootArrow.GetComponent<Arrow>();
        arrow.isFinalShot = isFianlShot;

        StartCoroutine(arrow.ShootLine(pos));
    }

    // Shoot2 애니메이션
    public void Shoot2()
    {
        shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowLine(dest);
    }

    // Shoot3 애니메이션
    public void Shoot3()
    {
        shootStartPos = arrow.transform.position;
        arrow.SetActive(false);

        // 타일로쏠꺼냐(빈타일에도 화살이날라감)
        // 세방 쏠꺼냐
        // 몬스터 에게 쏠꺼냐
        var list = TileMaker.Instance.GetSkillRangedTiles(controller.command.target, controller.command.skill.SkillTableElem.range);
        var monsterList = TileMaker.Instance.GetTargetList(controller.command.target, controller.command.skill.SkillTableElem.range);
        foreach (var monster in monsterList)
        {
            var renderer = monster.GetComponentInChildren<SkinnedMeshRenderer>();
            var center = renderer.bounds.center;
            var dest = center;
            ShootArrowCurve(dest);
        }
    }

    // Shoot4 애니메이션
    public void Shoot4()
    {
        shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowLine(dest);
    }
    public void FinalShoot()
    {
        shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowLine(dest, true);
    }


}