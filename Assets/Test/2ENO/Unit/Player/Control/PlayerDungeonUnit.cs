using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class PlayerDungeonUnitData
{
    public int curRoomNumber = -1;
    public Vector3 curPlayerPosition;
    public void SetUnitData(PlayerDungeonUnit unitData)
    {
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
    private float speed = 7f;

    private int curRoomNumber = 0;

    public MoveTest playerMove;
    public bool isGatheringEnd;

    public bool IsCoMove
    {
        set
        { 
            isCoMove = value;
            playerMove.isCoMove = value;
        }
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
        curRoomNumber = playerData.curRoomNumber;
        transform.position = playerData.curPlayerPosition;
    }

    private void Start()
    {
        playerAnimation = gameObject.GetComponent<Animator>();
        multiTouch = GameManager.Manager.MultiTouch;
    }

    private void Update()
    {
        //// ����ĳ��Ʈ �ʿ�
        //// UI�� �ƴ� �� �����ϰԲ� ��������
        //// ù ��ġ �������θ� ����
        //var isRayCol = Physics.Raycast(Camera.main.ScreenPointToRay(multiTouch.PrimaryStartPos), out _, Mathf.Infinity);
        //if (!isCoMove && multiTouch.TouchCount > 0 && isRayCol)
        //{
        //    if (EventSystem.current.IsPointerOverGameObject())
        //        return;
        //    isMove = false;
        //    // ���� ��ġ�ϰ� ���� �� �÷��̾�� �������� ���������� �Ǵ��ϴ� ���·� �����ϱ�..
        //    var touchXPos = Camera.main.ScreenToViewportPoint(multiTouch.PrimaryPos).x;
        //    var playerXPos = Camera.main.WorldToViewportPoint(transform.localPosition).x;
        //    if (playerXPos + 0.05f < touchXPos)
        //    {
        //        var pos = speed * Time.deltaTime * Vector3.forward;
        //        transform.position += pos;
        //        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        //        isMove = true;
        //    }
        //    else if (playerXPos - 0.05f > touchXPos)
        //    {
        //        var pos = speed * Time.deltaTime * -Vector3.forward;
        //        transform.position += pos;
        //        transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        //        isMove = true;
        //    }
        //    AnimationChange(isMove);
        //}
    }

    public void CoMoveStop()
    {
        isCoMove = false;
        playerMove.isCoMove = false;
        isGatheringEnd = false;
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
    private void PlayGatherAniEnd()
    {
        isGatheringEnd = true;
    }
}
