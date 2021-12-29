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


public class MonsterStats : MonoBehaviour
{
    public Vector2 tilePos;
    public bool istrapped;

    public TileMaker tilemaker;

    public void Init(Vector2 pos)
    {
        tilePos = pos;
        Debug.Log(tilePos);

        var MakerPos = new Tiles();
        MakerPos = tilemaker.GetTile(new Vector2((int)pos.x, (int)pos.y));
        gameObject.transform.position = MakerPos.transform.position;
    }
}
