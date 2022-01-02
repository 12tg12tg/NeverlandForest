using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
