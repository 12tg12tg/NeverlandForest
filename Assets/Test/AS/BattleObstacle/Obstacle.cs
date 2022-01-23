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
    [Header("설치물 타입")]
    public ObstacleType type;
    public GameObject prefab;

    [Header("장애물 속성")]
    public int trapDamage;
    public int duration;
    public int numberOfInstallation;
    [SerializeField]
    public List<Obstacle> pair = new List<Obstacle>();

    [Header("장벽 체력")]
    public int hp;

    public Obstacle() { }

    public Obstacle(Obstacle obstacle) 
    {
        // 깊은 복사
        type = obstacle.type;
        prefab = obstacle.prefab;
        duration = obstacle.duration;
        trapDamage = obstacle.trapDamage;
        hp = obstacle.hp;
        numberOfInstallation = obstacle.numberOfInstallation;
        pair = obstacle.pair;
    }

    public Obstacle(ObstacleType type, GameObject prefab, int duration, int trapDamage, int hp)
    {
        this.type = type;
        this.prefab = prefab;
        this.duration = duration;
        this.trapDamage = trapDamage;
        this.hp = hp;
    }
}
