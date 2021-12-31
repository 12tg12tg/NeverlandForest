using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDungeonUnit : UnitBase
{
    private Animator playerAnimation;
    private bool isMove;
    public bool isCoMove;
    public PlayerMoveAnimation curAnimation;

    private void Start()
    {
        playerAnimation = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        // 테스트용, 던전 이동씬에서만 동작!
        if (!isCoMove)
        {
            isMove = false;
            if (Input.GetKey(KeyCode.D))
            {
                var pos = Vector3.forward * 7f * Time.deltaTime;
                transform.position += pos;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                isMove = true;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                var pos = -Vector3.forward * 7f * Time.deltaTime;
                transform.position += pos;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                isMove = true;
            }
            AnimationChange(isMove);
        }
    }

    public void CoMoveStop()
    {
        isCoMove = false;
    }

    public void AnimationChange(object isMove)
    {
        if ((bool)isMove)
        {
            if (curAnimation == PlayerMoveAnimation.Walk)
                return;
            curAnimation = PlayerMoveAnimation.Walk;
            playerAnimation.SetTrigger("Walk");
        }
        else if (!(bool)isMove)
        {
            if (curAnimation == PlayerMoveAnimation.Idle)
                return;
            curAnimation = PlayerMoveAnimation.Idle;
            playerAnimation.SetTrigger("Idle");
        }
    }
}
