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

        currentIndex = Vector2.right; // �ε��� 1,0 �̶�� �ǹ�.. ���� ���� ���� ���
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
        // �÷��̾ �̵��ϸ� ��� �� Ȯ�� ����(��ĭ ������ �̵� �� 10����, ���󹰿� ������ �� 10����)
        // ������ �÷��̾ �߰� �� Ȯ�� ����
        bool isForward = false;
        // ���� ��ġ���� �ڷ� �̵�, 2ĭ ������ �̵�, �밢�� 2ĭ �̵� ����
        if (index.y <= currentIndex.y - 1 ||
            index.y >= currentIndex.y + 2 ||
            Mathf.Abs(index.x - currentIndex.x) > 1)
            return;

        // index�� y �� �񱳸� ���ؼ� ������ ��ĭ ���� �ߴ��� �Ǵ� ����
        if (index.y.Equals(currentIndex.y + 1) && coMove == null)
        {
            isForward = true;
            // ������ ����ĥ Ȯ�� ��
            EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape, true);
            // ���Ȯ�� ��
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
        playerAnimation.SetTrigger((bool)isMove ? "Walk" : "Idle");
    }
}
