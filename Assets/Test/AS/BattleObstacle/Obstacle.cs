using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    None,
    Lasso,
    BoobyTrap,
    WoodenTrap,
    ThornTrap,
    Barrier,
}

[Serializable]
public class Obstacle
{
    [Header("��ġ�� Ÿ��")]
    public ObstacleType type;
    public GameObject prefab;

    [Header("Ʈ�� ������ �� ���� �� ��")]
    public float trapDamage;
    public int duration;

    [Header("�庮 ü��")]
    public float hp;

    public Obstacle() { }

    public Obstacle(Obstacle obstacle) 
    {
        // ���� ����
        type = obstacle.type;
        prefab = obstacle.prefab;
        duration = obstacle.duration;
        trapDamage = obstacle.trapDamage;
        hp = obstacle.hp;
    }

    public Obstacle(ObstacleType type, GameObject prefab, int duration, float trapDamage, float hp)
    {
        this.type = type;
        this.prefab = prefab;
        this.duration = duration;
        this.trapDamage = trapDamage;
        this.hp = hp;
    }
}
