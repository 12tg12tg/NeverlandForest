using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHuntingUnit : UnitBase
{
    private Coroutine coMove;
    private PlayerStats playerStat;

    private Animator playerAnimation;

    private Vector2 currentIndex;
    public Vector2 CurrentIndex
    {
        get => currentIndex;
    }


    private void Start()
    {
        playerAnimation = gameObject.GetComponent<Animator>();
        playerStat = gameObject.GetComponent<PlayerStats>();

        currentIndex = Vector2.right; // 인덱스 1,0 이라는 의미.. 최초 시작 왼쪽 가운데
        Utility.arg1Event += AnimationChange;
    }

    private void OnDestroy()
    {
        Utility.arg1Event -= AnimationChange;
    }


    //Vector2 index, Vector3 pos
    public void Move(Vector2 index, Vector3 pos, bool isOnBush)
    {
        AnimationChange(true);
        // 플레이어가 이동하면 사냥 할 확률 증가(한칸 앞으로 이동 시 10프로, 은폐물에 숨었을 때 10프로)
        // 동물이 플레이어를 발견 할 확률 증가
        bool isForward = false;
        // 현재 위치에서 뒤로 이동, 2칸 앞으로 이동, 대각선 2칸 이동 막음
        if (index.y <= currentIndex.y - 1 ||
            index.y >= currentIndex.y + 2 ||
            Mathf.Abs(index.x - currentIndex.x) > 1)
            return;

        // index의 y 값 비교를 통해서 앞으로 한칸 전진 했는지 판단 가능
        if (index.y.Equals(currentIndex.y + 1))
        {
            isForward = true;
            // 동물이 도망칠 확률 업
            EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape, true);
            // 사냥확률 업
            EventBus<HuntingEvent>.Publish(HuntingEvent.PlayerMove, isForward, isOnBush);
        }
        else
        {
            EventBus<HuntingEvent>.Publish(HuntingEvent.PlayerMove, isForward, isOnBush);
        }
        var newPos = pos/* + new Vector3(0f, 1f, 0f)*/;

        currentIndex = coMove == null ? index : currentIndex;
        coMove ??= StartCoroutine(Utility.CoTranslate2(transform, transform.position, newPos, 1f, new Vector3(0f,180f,0f)
            ,() => coMove = null));

        EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape);
    }

    private void AnimationChange(object isMove)
    {
        if ((bool)isMove)
        {
            playerAnimation.SetTrigger("Walk");
        }
        else if (!(bool)isMove)
        {
            playerAnimation.SetTrigger("Idle");
        }
    }
}
