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
    [Header("유저의 타일 클릭을 기다리는 상황인지 확인")] public bool isWaitingTileSelect;

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
        if (tile.Units_UnitCount() == 2) // 만의 하나 추가할 수 없는 상황이 온다면
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
                // 원래 놓인 몬스터가 뒤쪽이었던 경우.
                //  1) 새 몬스터를 앞포지션으로 이동시킴.

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                StartCoroutine(CoMoveMonster(monsterUnit, frontDest,
                    moveStartAction, moveEndAction, rotateFoward));
            }
            else
            {
                // 원래 놓인 몬스터가 앞쪽이거나 중앙이었던 경우.
                //  1) 새 몬스터를 뒷포지션으로 이동시킴.
                //  2) 기존 몬스터를 앞포지션으로 이동시킴.

                var behindDest = tile.BehindPos;
                behindDest.y = tile.BehindMonster.transform.position.y;

                StartCoroutine(CoMoveMonster(monsterUnit, behindDest,
                    moveStartAction, moveEndAction, rotateFoward));

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                if (frontDest != tile.FrontMonster.transform.position)
                    StartCoroutine(CoMoveMonster(tile.FrontMonster, frontDest,
                        tile.FrontMonster.PlayMoveAnimation, tile.FrontMonster.PlayIdleAnimation));
            }
        }
        else if (tile.Units_UnitCount() == 1)
        {
            var dest = tile.CenterPos;
            dest.y = monsterUnit.transform.position.y;
            StartCoroutine(CoMoveMonster(monsterUnit, dest, moveStartAction, moveEndAction, rotateFoward));
        }
    }

    public IEnumerator CoMoveMonster(MonsterUnit unit, Vector3 dest, UnityAction startAc, UnityAction endAc, bool haveToRotate = true)
    {
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
    }

    public void DisplayMonsterTile(PlayerSkillTableElem skill)
    {
        targetTiles = tileMaker.GetMonsterTiles(skill);
        if(targetTiles.Count() == 0)
        {
            bm.uiLink.PrintCaution("스킬 범위 내에 몬스터가 없습니다.", 1f, 1f, 
                () => BottomUIManager.Instance.curSkillButton?.Cancle());
        }
        foreach (var tile in targetTiles)
        {
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
