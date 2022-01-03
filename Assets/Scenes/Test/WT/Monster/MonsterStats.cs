using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterDirection
{
    Forward,
    Backward,
    Left,
    Right,
    Forward_Left,
    Forward_Right
}


public class MonsterStats : UnitBase , IAttackable
{
    public Vector2 tilePos;
    public bool istrapped;

    public TileMaker tilemaker;

    private void Start()
    {
        //tilePos = tilemaker.allTiles[2,7].index;
        Debug.Log(tilePos);
        Hp = 1000;
        var MakerPos = new Tiles();
        MakerPos = tilemaker.GetTile(tilePos);
        gameObject.transform.position = MakerPos.transform.position;
    }

    public void Init(Vector2 pos)
    {
        //tilePos = pos;
        //Debug.Log(tilePos);

        //var MakerPos = new Tiles();
        //MakerPos = tilemaker.GetTile(new Vector2((int)pos.x, (int)pos.y));
        //gameObject.transform.position = MakerPos.transform.position;
    }

    public void OnAttacked(UnitBase attacker)
    {
        Debug.Log($"{tilePos} ��ġ�� ���Ͱ� ���ݹ޾ҽ��ϴ�");
        Debug.Log($"���ݹޱ��� hp :{Hp}");
        Hp -= attacker.Atk;
        Debug.Log($"���ݹ����� hp :{Hp}");
    }
}
