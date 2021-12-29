using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDungeonUnit : UnitBase
{
    private Animator playerAnimation;
    private bool isMove;
    private PlayerMoveAnimation curAnimation;

    private void Start()
    {
        playerAnimation = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        // 테스트용, 던전 이동씬에서만 동작!
        isMove = false;
        if (Input.GetKey(KeyCode.D))
        {
            var pos = Vector3.forward * 5f * Time.deltaTime;
            transform.position += pos;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            isMove = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            var pos = -Vector3.forward * 5f * Time.deltaTime;
            transform.position += pos;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
            isMove = true;
        }
        AnimationChange(isMove);
    }

    private void AnimationChange(bool isMove)
    {
        if (isMove && (curAnimation != PlayerMoveAnimation.Walk))
        {
            curAnimation = PlayerMoveAnimation.Walk;
            playerAnimation.SetTrigger("Walk");
        }
        else if (!isMove && (curAnimation != PlayerMoveAnimation.Idle))
        {
            curAnimation = PlayerMoveAnimation.Idle;
            playerAnimation.SetTrigger("Idle");
        }
    }
}
