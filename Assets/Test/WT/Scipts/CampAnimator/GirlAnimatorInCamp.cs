using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlAnimatorInCamp : MonoBehaviour
{
    public Animator girlAnimator;
    private float timer=0f;
    private float idleTime = 10f;
    // Update is called once per frame
    void Update()
    {
        if (timer==0f)
            ChangeAnimation();
        timer += Time.deltaTime;
        if (timer>idleTime)
        {
            AllReset();
            timer = 0f;
        }
    }

    public void ChangeAnimation()
    {
        var rand = Random.Range(0, 4); // 0,1,23
        if (rand ==0)
        {
            girlAnimator.SetBool("Angry", true);
        }
        else if (rand == 1)
        {
            girlAnimator.SetBool("Sitting", true);
        }
        else if(rand == 2)
        {
            girlAnimator.SetBool("SittingTalk", true);
        }
        else
        {
            girlAnimator.SetBool("Bored", true);
        }
    }
    public void AllReset()
    {
        girlAnimator.SetBool("Angry", false);
        girlAnimator.SetBool("Sitting", false);
        girlAnimator.SetBool("SittingTalk", false);
        girlAnimator.SetBool("Bored", false);
    }

}
