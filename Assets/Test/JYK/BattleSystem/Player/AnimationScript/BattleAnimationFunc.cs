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

    // Attack �ִϸ��̼�
    public void Hit()
    {
        actionState.isAttackMotionEnd = true;
    }

    // Shoot �ִϸ��̼�
    public void ArrowCreate() // ��ɲ��� ���� �ڷ� ���� �� ȭ���� ���� �� ó�� �ϱ� ���� �ʿ��� ��
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
        shootArrow.transform.position = shootStartPos;
        var arrow = shootArrow.GetComponent<Arrow>();
        StartCoroutine(arrow.Shoot(pos, true));
    }

    public void ShootArrowLine(Vector3 pos, bool isFianlShot = false)
    {
        var shootArrow = ProjectilePool.Instance.GetObject(ProjectileTag.HunterArrow);
        shootArrow.transform.position = shootStartPos;

        var arrow = shootArrow.GetComponent<Arrow>();
        arrow.isFinalShot = isFianlShot;

        StartCoroutine(arrow.ShootLine(pos));
    }

    // Shoot2 �ִϸ��̼�
    public void Shoot2()
    {
        if (shootStartPos == Vector3.zero)
            shootStartPos = arrow.transform.position;
        arrow.SetActive(false);
        var dest = TileMaker.Instance.GetTile(controller.command.target).CenterPos;
        dest.y += 2f;
        ShootArrowLine(dest);
    }

    // Shoot3 �ִϸ��̼�
    public void Shoot3()
    {
        if (shootStartPos == Vector3.zero)
            shootStartPos = arrow.transform.position;
        arrow.SetActive(false);

        // Ÿ�Ϸν򲨳�(��Ÿ�Ͽ��� ȭ���̳���)
        // ���� �򲨳�
        // ���� ���� �򲨳�
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

    // Shoot4 �ִϸ��̼�
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

    // Light �ִϸ��̼�
    public void Light()
    {
        StartCoroutine(CoLightUp());
    }

    public void Burn()
    {
        StartCoroutine(CoLightDown());
        var command = controller.command;
        var list = TileMaker.Instance.GetTargetList(command.target, command.skill.SkillTableElem.range);
        foreach (var monster in list)
        {
            var go = ProjectilePool.Instance.GetObject(ProjectileTag.LightExplosion);
            var pos = go.transform.position = monster.transform.position;
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

        actionState.isAttackMotionEnd = true;
        Debug.Log($"���� ���Ͼ� {curLanternCount}���� {Vars.UserData.uData.LanternCount}�� ������.");
    }
}