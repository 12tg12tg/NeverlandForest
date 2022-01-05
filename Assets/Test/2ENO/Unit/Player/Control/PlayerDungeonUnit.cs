using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDungeonUnitData
{
    public PlayerMoveAnimation curAnimation;
    public int curRoomNumber = 0;
    public Vector3 curPlayerPosition;
    public void SetUnitData(PlayerDungeonUnit unitData)
    {
        curAnimation = unitData.CurAnimation;
        curRoomNumber = unitData.CurRoomNumber;
        curPlayerPosition = unitData.transform.position;
    }
}
public class PlayerDungeonUnit : UnitBase
{
    private Animator playerAnimation;
    private bool isMove;
    private PlayerMoveAnimation curAnimation;
    private bool isCoMove;
    private MultiTouch multiTouch;

    private int curRoomNumber = 0;
    public bool IsCoMove
    {
        set => isCoMove = value;
        get => isCoMove;
    }
    public int CurRoomNumber
    {
        set => curRoomNumber = value;
        get => curRoomNumber;
    }
    public PlayerMoveAnimation CurAnimation
    {
        get => curAnimation;
    }

    public void SetPlayerData(PlayerDungeonUnitData playerData)
    {
        curAnimation = playerData.curAnimation;
        curRoomNumber = playerData.curRoomNumber;
        transform.position = playerData.curPlayerPosition;
    }

    private void Start()
    {
        playerAnimation = gameObject.GetComponent<Animator>();
        multiTouch = GameManager.Manager.MultiTouch;
    }

    void Update()
    {
        // 테스트용, 던전 이동씬에서만 동작!
        if (!isCoMove && multiTouch.TouchCount > 0)
        {
            isMove = false;
            // 내가 터치한 지점이 플레이어보다 왼쪽인지 오른쪽인지 판단하는 형태로 구현하기..
            var playerXPos = Camera.main.WorldToViewportPoint(transform.localPosition).x;
            var touchXPos = Camera.main.ScreenToViewportPoint(multiTouch.PrimaryPos).x;
            if (playerXPos < touchXPos)
            {
                var pos = Vector3.forward * 7f * Time.deltaTime;
                transform.position += pos;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                isMove = true;
            }
            else if (playerXPos > touchXPos)
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

    public void AnimationChange(bool isMove)
    {
        if (isMove)
            PlayWalkAnimation();
        else
            PlayIdleAnimation();
    }

    private void PlayWalkAnimation()
    {
        if (curAnimation == PlayerMoveAnimation.Walk)
            return;
        curAnimation = PlayerMoveAnimation.Walk;
        playerAnimation.SetTrigger("Walk");
    }

    private void PlayIdleAnimation()
    {
        if (curAnimation == PlayerMoveAnimation.Idle)
            return;
        curAnimation = PlayerMoveAnimation.Idle;
        playerAnimation.SetTrigger("Idle");
    }
}
