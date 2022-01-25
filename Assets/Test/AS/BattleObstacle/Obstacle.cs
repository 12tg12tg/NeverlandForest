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

    [Header("��ֹ� �Ӽ�")]
    public int trapDamage;
    public int duration;
    public int numberOfInstallation;
    public List<Obstacle> pair = new List<Obstacle>();


    [Header("�庮 ü��")]
    public int hp;

    public Obstacle() { }

    public Obstacle(Obstacle obstacle) 
    {
        // ���� ����
        type = obstacle.type;
        prefab = obstacle.prefab;
        duration = obstacle.duration;
        trapDamage = obstacle.trapDamage;
        hp = obstacle.hp;
        numberOfInstallation = obstacle.numberOfInstallation;
        pair = obstacle.pair;
    }

    public Obstacle(ObstacleType type, GameObject prefab, int duration, int trapDamage, int hp, int numberOfInstallation, List<Obstacle> pair)
    {
        this.type = type;
        this.prefab = prefab;
        this.duration = duration;
        this.trapDamage = trapDamage;
        this.hp = hp;
        this.numberOfInstallation = numberOfInstallation;
        this.pair = pair;
    }
}
