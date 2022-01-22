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

    [Header("트랩 데미지 및 지속 턴 수")]
    public float trapDamage;
    public int duration;

    [Header("장벽 체력")]
    public float hp;

    public Obstacle() { }

    public Obstacle(Obstacle obstacle) 
    {
        // 깊은 복사
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
