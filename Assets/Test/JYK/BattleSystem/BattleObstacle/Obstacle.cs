using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDebuff
{
    // ����
    public MonsterUnit owner;
    public AllItemTableElem elem;

    // ����� ��꿵��
    public int trapDamage;
    public int duration;

    // �ð��� ����
    private ObstacleDebuff another;

    public MonsterUnit anotherUnit { get => another.owner; }

    public ObstacleDebuff(Obstacle obs, MonsterUnit owner)
    {
        this.owner = owner;
        elem = obs.elem;
        trapDamage = obs.elem.trapDamage;
        duration = obs.elem.duration;
        if(obs.another != null)
        {
            var otherSide = obs.another;
            otherSide.anotherDebuff = this;
            otherSide.another = null;
        }
        else if(obs.anotherDebuff != null)
        {
            another = obs.anotherDebuff;
        }
        obs.Release();
    }
}

[Serializable]
public class Obstacle : MonoBehaviour
{
    [Header("��ġ�� Ÿ��")]
    public TrapTag type;
    public AllItemTableElem elem;

    [Header("��ֹ� �Ӽ�")]
    public Obstacle another;                // ���� �ݴ����� �����ִ� ��� ����.
    public ObstacleDebuff anotherDebuff;    // �ݴ����� ���� �ɸ� ��� ����.

    [Header("�庮 ü��")]
    public int hp;

    private void Start()
    {
        var elems = DataTableManager.GetTable<AllItemDataTable>().data.Values;
        foreach (var elem in elems)
        {
            if((elem as AllItemTableElem).obstacleType == type)
            {
                this.elem = elem as AllItemTableElem;
                break;
            }
        }
    }

    public void Init(Obstacle another = null)
    {
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
        another = null;
        anotherDebuff = null;
        TrapPool.Instance.ReturnObject(type, gameObject);
    }
}
