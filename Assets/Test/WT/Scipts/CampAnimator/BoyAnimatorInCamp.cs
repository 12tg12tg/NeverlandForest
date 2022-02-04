using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyAnimatorInCamp : MonoBehaviour
{
    public Animator boyAnimator;
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
            boyAnimator.SetBool("Streching", true);
        }
        else if (rand == 1)
        {
            boyAnimator.SetBool("Sitting", true);
        }
        else if (rand == 2)
        {
            boyAnimator.SetBool("SittingTalk", true);
        }
        else
        {
            boyAnimator.SetBool("Bored", true);
        }
    }
    public void AllReset()
    {
        boyAnimator.SetBool("Streching", false);
        boyAnimator.SetBool("Sitting", false);
        boyAnimator.SetBool("SittingTalk", false);
        boyAnimator.SetBool("Bored", false);
    }

}
