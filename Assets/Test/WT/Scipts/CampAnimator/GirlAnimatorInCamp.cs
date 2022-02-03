using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlAnimatorInCamp : MonoBehaviour
{
    public Animator girlAnimator;
    private float timer=0f;
    private float resetTime=5f;
    private float idleTime = 10f;
    // Update is called once per frame
    void Update()
    {
        if (timer==0f)
            ChangeAnimation();
        timer += Time.deltaTime;
        if (timer> resetTime)
        {
            AllAnimationReset();
        }
        if (timer>idleTime)
        {
            timer = 0f;
        }
    }

    public void ChangeAnimation()
    {
        var rand = Random.Range(0, 3); // 0,1,2
        if (rand ==0)
        {
            girlAnimator.SetBool("Angry", true);
        }
        else if (rand == 1)
        {
            girlAnimator.SetBool("Sitting", true);
        }
        else
        {
            girlAnimator.SetBool("SittingTalk", true);
        }
    }
    public void AllAnimationReset()
    {
        girlAnimator.SetBool("Angry", false);
        girlAnimator.SetBool("Sitting", false);
        girlAnimator.SetBool("SittingTalk", false);
    }
}
