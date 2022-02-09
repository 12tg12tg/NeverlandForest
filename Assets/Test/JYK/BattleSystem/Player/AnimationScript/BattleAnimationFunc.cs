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
    public Light lanternLight;

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
        if(shootStartPos == Vector3.zero)
            shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowCurve(dest);
    }

    public void ShootArrowCurve(Vector3 pos)
    {
        var shootArrow = ProjectilePool.Instance.GetObject(ProjectileTag.HunterArrow);
        shootArrow.transform.SetParent(BattleManager.Instance.projectileParent);
        shootArrow.transform.position = shootStartPos;
        var arrow = shootArrow.GetComponent<Arrow>();
        StartCoroutine(arrow.Shoot(pos, true));
    }

    public void ShootArrowLine(Vector3 pos, bool isFianlShot = false)
    {
        var shootArrow = ProjectilePool.Instance.GetObject(ProjectileTag.HunterArrow);
        shootArrow.transform.SetParent(BattleManager.Instance.projectileParent);
        shootArrow.transform.position = shootStartPos;

        var arrow = shootArrow.GetComponent<Arrow>();
        arrow.isFinalShot = isFianlShot;

        StartCoroutine(arrow.ShootLine(pos));
    }

    // Shoot2 애니메이션
    public void Shoot2()
    {
        if (shootStartPos == Vector3.zero)
            shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowLine(dest);
    }

    // Shoot3 애니메이션
    public void Shoot3()
    {
        if (shootStartPos == Vector3.zero)
            shootStartPos = arrow.transform.position;
        arrow.SetActive(false);

        // 타일로쏠꺼냐(빈타일에도 화살이날라감)
        // 세방 쏠꺼냐
        // 몬스터 에게 쏠꺼냐
        var monsterList = TileMaker.Instance.GetTargetList(controller.command.target, controller.command.skill.SkillTableElem.range, false);
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
        if (shootStartPos == Vector3.zero)
            shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowLine(dest);
    }
    public void FinalShoot()
    {
        if (shootStartPos == Vector3.zero)
            shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowLine(dest, true);
    }

    // Light 애니메이션
    public void Light()
    {
        StartCoroutine(CoLightUp());
    }

    public void Burn()
    {
        StartCoroutine(CoLightDown());
        var command = controller.command;
        var isDrag = BattleManager.Instance.inputLink.isLastInputDrag;
        var list = TileMaker.Instance.GetTargetList(command.target, command.skill.SkillTableElem.range, isDrag);
        foreach (var monster in list)
        {
            var go = ProjectilePool.Instance.GetObject(ProjectileTag.LightExplosion);
            go.transform.SetParent(BattleManager.Instance.projectileParent);
            var pos = monster.transform.position;
            var ren = monster.GetComponentInChildren<SkinnedMeshRenderer>();
            var maxY = ren.bounds.max.y;
            pos.y = maxY;

            go.transform.position = pos;
            var particle = go.GetComponent<Particle>();
            particle.Init();
        }
        actionState.isAttackMotionEnd = true;
    }

    private IEnumerator CoLightUp()
    {
        var time = 1f;
        var timer = 0f;
        var curRange = lanternLight.range;
        var destRange = curRange + 2f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;
            lanternLight.range = Mathf.Lerp(curRange, destRange, ratio);
            yield return null;
        }
        lanternLight.range = destRange;
    }
    private IEnumerator CoLightDown()
    {
        var time = 0.4f;
        var timer = 0f;
        var curRange = lanternLight.range;
        var destRange = curRange - 2f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;
            lanternLight.range = Mathf.Lerp(curRange, destRange, ratio);
            yield return null;
        }
        lanternLight.range = destRange;
    }
    public void Charge()
    {
        var chargeSkillElem = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>(BattleManager.Instance.costLink.skillID_chargeOil);
        var chargeAmount = chargeSkillElem.Damage;
        var curLanternCount = Vars.UserData.uData.LanternCount;
        ConsumeManager.FullingLantern(chargeAmount);
        BattleManager.Instance.uiLink.UpdateLanternRange();

        actionState.isAttackMotionEnd = true;
        Debug.Log($"원래 랜턴양 {curLanternCount}에서 {Vars.UserData.uData.LanternCount}로 충전됨.");
    }
}