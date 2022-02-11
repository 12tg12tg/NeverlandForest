using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class ObstacleDebuff
{
    // 주인
    public MonsterUnit owner;
    public AllItemTableElem elem;

    // 디버프 계산영역
    public int trapDamage;
    public int duration;

    // 올가미 관련
    public Obstacle oppositeSnare;
    public ObstacleDebuff another;

    public MonsterUnit anotherUnit { get => another?.owner; }

    public ObstacleDebuff(Obstacle obs, MonsterUnit owner)
    {
        this.owner = owner;
        elem = obs.elem;
        trapDamage = obs.elem.damage;

        if(obs.elem.obstacleType == TrapTag.WoodenTrap)
        {
            duration = 3;
        }
        else if(obs.elem.obstacleType == TrapTag.ThornTrap)
        {
            duration = 3;
        }
        else if(obs.elem.obstacleType == TrapTag.Snare)
        {
            duration = 999;
        }

        if(obs.another != null)
        {
            oppositeSnare = obs.another;
            oppositeSnare.anotherDebuff = this;
            oppositeSnare.another = null;
        }
        else if(obs.anotherDebuff != null)
        {
            another = obs.anotherDebuff;
            another.another = this;
            oppositeSnare = null;
        }
        obs.Release();
    }
}

[Serializable]
public class Obstacle : MonoBehaviour, IPointerClickHandler
{
    [Header("설치 타일 확인")]
    public Tiles tile;

    [Header("설치물 타입 확인")]
    public TrapTag type;
    public AllItemTableElem elem;

    [Header("장애물 속성 확인")]
    public Obstacle another;                // 아직 반대쪽이 남아있는 경우 존재.
    public ObstacleDebuff anotherDebuff;    // 반대쪽이 먼저 걸린 경우 존재.

    [Header("장벽 체력 확인")]
    public int hp;

    private bool isInit;
    private SpriteRenderer sprite;

    private void Start()
    {
        var elems = DataTableManager.GetTable<AllItemDataTable>().data.Values;
        foreach (var elem in elems)
        {
            if((elem as AllItemTableElem).type.Equals("INSTALLATION")
                && (elem as AllItemTableElem).obstacleType == type)
            {
                this.elem = elem as AllItemTableElem;
                break;
            }
        }

        if(type == TrapTag.Snare)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (isInit && type == TrapTag.Snare)
        {
            var rotate = Quaternion.LookRotation(sprite.transform.position - Camera.main.transform.position);
            sprite.transform.rotation = rotate;
        }
    }

    public void Init(Tiles tile, Obstacle another = null)
    {
        isInit = true;
        this.tile = tile;
        switch (type)
        {
            case TrapTag.Snare:
                if (another != null)
                {
                    this.another = another;
                    another.another = this;
                }
                break;

            case TrapTag.BoobyTrap:
            case TrapTag.WoodenTrap:
            case TrapTag.ThornTrap:
                break;

            case TrapTag.Fence:
                hp = elem.obstacleHp;
                break;
        }
    }

    public void Release()
    {
        isInit = false;
        tile = null;
        another = null;
        anotherDebuff = null;
        TrapPool.Instance.ReturnObject(type, gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(BattleManager.Instance.FSM.curState == BattleState.Start
            && !TileMaker.Instance.IsWaitingToSelectTrapTile)
        {
            if (BattleManager.Instance.isTutorial)
                return;

            if (type == TrapTag.Snare)
            {
                another.tile.obstacle = null;
                another.Release();
            }

            tile.obstacle = null;
            Release();
            var newItem = new DataAllItem(elem);
            newItem.OwnCount = 1;
            Vars.UserData.AddItemData(newItem);

            BottomUIManager.Instance.ItemListInit();
        }
    }
}
