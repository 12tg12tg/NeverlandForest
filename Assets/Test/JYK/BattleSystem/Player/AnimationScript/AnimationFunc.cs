using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFunc : MonoBehaviour
{
    public PlayerAction actionState;
    public void Hit()
    {
        actionState.isAttackMotionEnd = true;
        Debug.Log("Animation Hit Func");
    }
}
