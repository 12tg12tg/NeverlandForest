using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntPlayer : UnitBase
{
    private Coroutine coMove;
    public GameObject shootArrow;
    public Animator playerAnimation;

    private Vector2 currentIndex;
    public Vector2 CurrentIndex => currentIndex;

    private void Start()
    {
        currentIndex = Vector2.right; // �ε��� 1,0 �̶�� �ǹ�.. ���� ���� ���� ���
    }

    public void Move(Vector2 index, Vector3 pos, bool isOnBush)
    {
        // �÷��̾ �̵��ϸ� ��� �� Ȯ�� ����(��ĭ ������ �̵� �� 10����, ���󹰿� ������ �� 10����)
        // ������ �÷��̾ �߰� �� Ȯ�� ����
        var isForward = false;
        // ���� ��ġ���� �ڷ� �̵�, 2ĭ ������ �̵�, ���� ĭ, �밢�� 2ĭ �̵� ����
        if (index.y <= currentIndex.y - 1 ||
            index.y >= currentIndex.y + 2 ||
            index.Equals(currentIndex) ||
            Mathf.Abs(index.x - currentIndex.x) > 1)
            return;

        // index�� y �� �񱳸� ���ؼ� ������ ��ĭ ���� �ߴ��� �Ǵ� ����
        if (index.y.Equals(currentIndex.y + 1) && coMove == null)
        {
            isForward = true;
            // ������ ����ĥ Ȯ�� ��
            EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape, true);
        }
        EventBus<HuntingEvent>.Publish(HuntingEvent.PlayerMove, isForward, isOnBush);

        if (coMove == null)
        {
            playerAnimation.SetTrigger("Walk");
            currentIndex = index;
        }
        coMove ??= StartCoroutine(Utility.CoTranslateLookFoward(transform, transform.position, pos, 1f, () =>
        {
            playerAnimation.SetTrigger("Idle");
            transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            coMove = null;
            EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape);
        }));
    }
    public void ShootAnimation(Vector3 animalPos)
    {
        playerAnimation.SetTrigger("ShootingArrow");
        transform.LookAt(animalPos);
    }
}
