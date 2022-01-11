using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHuntingUnit : UnitBase
{
    private Coroutine coMove;

    private Animator playerAnimation;

    private Vector2 currentIndex;
    public Vector2 CurrentIndex => currentIndex;

    private void Start()
    {
        playerAnimation = gameObject.GetComponent<Animator>();

        currentIndex = Vector2.right; // �ε��� 1,0 �̶�� �ǹ�.. ���� ���� ���� ���
    }

    //Vector2 index, Vector3 pos
    public void Move(Vector2 index, Vector3 pos, bool isOnBush)
    {
        // �÷��̾ �̵��ϸ� ��� �� Ȯ�� ����(��ĭ ������ �̵� �� 10����, ���󹰿� ������ �� 10����)
        // ������ �÷��̾ �߰� �� Ȯ�� ����
        bool isForward = false;
        // ���� ��ġ���� �ڷ� �̵�, 2ĭ ������ �̵�, ���� ĭ, �밢�� 2ĭ �̵� ����
        if (index.y <= currentIndex.y - 1 ||
            index.y >= currentIndex.y + 2 ||
            index.Equals(currentIndex) ||
            Mathf.Abs(index.x - currentIndex.x) > 1)
            return;
        AnimationChange(true);

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

        if (coMove == null)
        {
            PlayWalkAnimation();
            currentIndex = index;
        }

        coMove ??= StartCoroutine(Utility.CoTranslateLookFoward(transform, transform.position, pos, 1f, () => AfterMove()));

        EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape);
    }

    private void AnimationChange(object isMove)
    {
        playerAnimation.SetTrigger((bool)isMove ? "Walk" : "Idle");
    }

    private void PlayWalkAnimation()
    {
        playerAnimation.SetTrigger("Walk");
    }

    private void PlayIdleAnimation()
    {
        playerAnimation.SetTrigger("Idle");
    }

    private void AfterMove()
    {
        PlayIdleAnimation();      
        transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        coMove = null;
    }
}
