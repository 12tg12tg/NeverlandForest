using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFunc : MonoBehaviour
{
    public PlayerAction actionState;
    public HuntingSystem huntingSystem;
    public GameObject arrow;

    public void Hit()
    {
        actionState.isAttackMotionEnd = true;
        Debug.Log("Animation Hit Func");
    }

    public void ArrowCreate()
    {
        arrow.SetActive(true);
    }

    public void Shoot()
    {
        arrow.SetActive(false);
        huntingSystem.Shooting();
    }
}
