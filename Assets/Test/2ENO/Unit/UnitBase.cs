using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    private int hp;
    private int atk;
    private int eva;
    private Vector2 pos;

    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
        }
    }

    public int Atk
    {
        get => atk;
        set
        {
            atk = value;
        }
    }

    public int Eva
    {
        get => eva;
        set
        {
            eva = value;
        }
    }

    public Vector2 Pos
    {
        get => pos;
        set => pos = value;
    }

    public Tiles CurTile
    {
        get => TileMaker.Instance.GetTile(Pos);
    }
}
