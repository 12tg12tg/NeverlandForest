using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleTile : MonoBehaviour
{
    private const float monsterSpeed = 10f;

    private BattleManager bm;
    private TileMaker tileMaker;
    private IEnumerable<Tiles> targetTiles;
    [Header("������ Ÿ�� Ŭ���� ��ٸ��� ��Ȳ���� Ȯ��")] public bool isWaitingTileSelect;

    private void Start()
    {
        bm = BattleManager.Instance;
        tileMaker = TileMaker.Instance;
    }

    public void SetUnitOnTile(Vector2 tilePos, UnitBase unit)
    {
        var tile = tileMaker.GetTile(tilePos);

        tile.Units_UnitAdd(unit);
        unit.Pos = tilePos;

        var dest = tile.CenterPos;
        dest.y = unit.transform.position.y;
        unit.transform.position = dest;
    }

    public void MoveUnitOnTile(Vector2 tilePos, MonsterUnit monsterUnit, UnityAction moveStartAction, UnityAction moveEndAction, bool rotateFoward = true)
    {
        var tile = tileMaker.GetTile(tilePos);
        if (tile.Units_UnitCount() == 2) // ���� �ϳ� �߰��� �� ���� ��Ȳ�� �´ٸ�
        {
            moveEndAction?.Invoke();
            return;
        }

        var preTile = monsterUnit.CurTile;
        preTile.RemoveUnit(monsterUnit);

        tile.Units_UnitAdd(monsterUnit);
        monsterUnit.Pos = tilePos;

        MonsterUnit alreadyPlacedMonster = (tile.FrontMonster == monsterUnit) ? tile.BehindMonster : tile.FrontMonster;
        bool isAlreadyBehind = tile.FrontMonster == monsterUnit;

        if (tile.Units_UnitCount() == 2)
        {
            if (isAlreadyBehind)
            {
                // ���� ���� ���Ͱ� �����̾��� ���.
                //  1) �� ���͸� ������������ �̵���Ŵ.

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                monsterUnit.moveCoroutine = StartCoroutine(CoMoveMonster(monsterUnit, frontDest,
                    moveStartAction, moveEndAction, rotateFoward));
                monsterUnit.afterMove = moveEndAction;
            }
            else
            {
                // ���� ���� ���Ͱ� �����̰ų� �߾��̾��� ���.
                //  1) �� ���͸� ������������ �̵���Ŵ.
                //  2) ���� ���͸� ������������ �̵���Ŵ.

                var behindDest = tile.BehindPos;
                behindDest.y = tile.BehindMonster.transform.position.y;

                monsterUnit.moveCoroutine = StartCoroutine(CoMoveMonster(monsterUnit, behindDest,
                    moveStartAction, moveEndAction, rotateFoward));
                monsterUnit.afterMove = moveEndAction;

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                if (frontDest != tile.FrontMonster.transform.position)
                {
                    tile.FrontMonster.moveCoroutine = StartCoroutine(CoMoveMonster(tile.FrontMonster, frontDest,
                        tile.FrontMonster.PlayMoveAnimation, tile.FrontMonster.PlayIdleAnimation));
                    tile.FrontMonster.afterMove = tile.FrontMonster.PlayIdleAnimation;
                }
            }
        }
        else if (tile.Units_UnitCount() == 1)
        {
            var dest = tile.CenterPos;
            dest.y = monsterUnit.transform.position.y;
            monsterUnit.moveCoroutine = StartCoroutine(CoMoveMonster(monsterUnit, dest, moveStartAction, moveEndAction, rotateFoward));
            monsterUnit.afterMove = moveEndAction;
        }
    }

    public void KickOutUnitOnTile(Vector2 tilePos, MonsterUnit monsterUnit, UnityAction moveStartAction, UnityAction moveEndAction)
    {
        var tile = tileMaker.GetTile(tilePos);
        if (tile.Units_UnitCount() == 2) // ���� �ϳ� �߰��� �� ���� ��Ȳ�� �´ٸ�
        {
            moveEndAction?.Invoke();
            return;
        }
        //========================================================
        switch (tile.Units_UnitCount())
        {
            case 0: Debug.Log($"{monsterUnit.BaseElem.Name}=== 0���̿�"); break;
            case 1: Debug.Log($"{monsterUnit.BaseElem.Name}=== 1���̿�"); break;
            case 2: Debug.Log($"{monsterUnit.BaseElem.Name}=== 2���̿�"); break;
        }
        //========================================================
        var preTile = monsterUnit.CurTile;
        preTile.RemoveUnit(monsterUnit);

        tile.Units_UnitAdd(monsterUnit);
        monsterUnit.Pos = tilePos;

        MonsterUnit alreadyPlacedMonster = (tile.FrontMonster == monsterUnit) ? tile.BehindMonster : tile.FrontMonster;
        bool isAlreadyBehind = tile.FrontMonster == monsterUnit;

        if (tile.Units_UnitCount() == 2)
        {
            if (isAlreadyBehind)
            {
                // ���� ���� ���Ͱ� �����̾��� ���.
                //  1) �� ���͸� ������������ �̵���Ŵ.

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                monsterUnit.moveCoroutine = StartCoroutine(CoKickOutMonster(monsterUnit, frontDest,
                    moveStartAction, moveEndAction));
                monsterUnit.afterMove = moveEndAction;
            }
            else
            {
                // ���� ���� ���Ͱ� �����̰ų� �߾��̾��� ���.
                //  1) �� ���͸� ������������ �̵���Ŵ.
                //  2) ���� ���͸� ������������ �̵���Ŵ.

                var behindDest = tile.BehindPos;
                behindDest.y = tile.BehindMonster.transform.position.y;

                monsterUnit.moveCoroutine = StartCoroutine(CoKickOutMonster(monsterUnit, behindDest,
                    moveStartAction, moveEndAction));
                monsterUnit.afterMove = moveEndAction;

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                if (frontDest != tile.FrontMonster.transform.position)
                {
                    tile.FrontMonster.moveCoroutine = StartCoroutine(CoMoveMonster(tile.FrontMonster, frontDest,
                        tile.FrontMonster.PlayMoveAnimation, tile.FrontMonster.PlayIdleAnimation));
                    tile.FrontMonster.afterMove = tile.FrontMonster.PlayIdleAnimation;
                }
            }
        }
        else if (tile.Units_UnitCount() == 1)
        {
            var dest = tile.CenterPos;
            dest.y = monsterUnit.transform.position.y;
            monsterUnit.moveCoroutine = StartCoroutine(CoKickOutMonster(monsterUnit, dest, moveStartAction, moveEndAction));
            monsterUnit.afterMove = moveEndAction;
        }
    }

    public IEnumerator CoKickOutMonster(MonsterUnit unit, Vector3 dest, UnityAction startAc, UnityAction endAc)
    {
        if(unit.moveCoroutine != null)
        {
            StopCoroutine(unit.moveCoroutine);
            unit.afterMove?.Invoke();
        }

        var startRot = Quaternion.LookRotation(unit.transform.forward);
        var destRot = Quaternion.LookRotation(unit.transform.position - dest);

        if (Quaternion.Angle(startRot, destRot) > 0f)
            yield return StartCoroutine(Utility.CoRotate(unit.transform, startRot, destRot, 0.3f));

        yield return new WaitForSeconds(0.3f);

        var speed = monsterSpeed * 2;

        startAc?.Invoke();
        yield return StartCoroutine(Utility.CoTranslate(unit.transform, dest, speed, 0.3f, null, true));
        endAc?.Invoke();

        if (Quaternion.Angle(startRot, destRot) > 0f)
            yield return StartCoroutine(Utility.CoRotate(unit.transform, destRot, startRot, 0.3f));

        unit.moveCoroutine = null;
        unit.afterMove = null;
    }

    public IEnumerator CoMoveMonster(MonsterUnit unit, Vector3 dest, UnityAction startAc, UnityAction endAc, bool haveToRotate = true)
    {
        if (unit.moveCoroutine != null)
        {
            StopCoroutine(unit.moveCoroutine);
            unit.afterMove?.Invoke();
        }

        var startRot = Quaternion.LookRotation(unit.transform.forward);
        var destRot = Quaternion.LookRotation(dest - unit.transform.position);

        if (haveToRotate && Quaternion.Angle(startRot, destRot) > 0f)
            yield return StartCoroutine(Utility.CoRotate(unit.transform, startRot, destRot, 0.3f));

        yield return new WaitForSeconds(0.3f);

        var speed = (haveToRotate) ? monsterSpeed : monsterSpeed * 2;

        startAc?.Invoke();
        yield return StartCoroutine(Utility.CoTranslate(unit.transform, dest, speed, 0.3f, null, true));
        endAc?.Invoke();

        if (haveToRotate && Quaternion.Angle(startRot, destRot) > 0f)
            yield return StartCoroutine(Utility.CoRotate(unit.transform, destRot, startRot, 0.3f));

        unit.moveCoroutine = null;
        unit.afterMove = null;
    }

    public void DisplaySkillTile(PlayerSkillTableElem skill)
    {
        targetTiles = tileMaker.GetSKillTiles(skill);

        if(targetTiles.Count() == 0)
        {
            bm.uiLink.PrintCaution("��ų ���� ���� ���Ͱ� �����ϴ�.", 1f, 0.5f, 
                () =>
                {
                    BottomUIManager.Instance.curSkillButton?.Cancle();
                    if (bm.dragLink.isDrag)
                        bm.dragLink.Release();
                });
        }

        foreach (var tile in targetTiles)
        {
            if (skill.id == bm.costLink.skillID_chargeOil) // ���� ����
                tile.HighlightCanConsumeSign();
            else
                tile.HighlightCanAttackSign(skill.range);
        }
    }

    public void UndisplayMonsterTile()
    {
        tileMaker.SetAllTileSoftClear();
    }

    public bool IsVaildTargetTile(Tiles tile)
    {
        return targetTiles.Contains(tile);
    }

    public void ReadyTileClick()
    {
        isWaitingTileSelect = true;
    }

    public void EndTileClick()
    {
        isWaitingTileSelect = false;
    }
}
