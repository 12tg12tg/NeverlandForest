using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntPlayer : UnitBase
{
    [Header("Hunter")]
    public Arrow shootArrow;
    public GameObject hunter;
    public Animator hunterAnimation;
    public GameObject bowHand;
    public GameObject bowBack;
    private Coroutine coHunterMove;
    private Vector2 currentIndex;
    public Vector2 CurrentIndex => currentIndex;

    [Header("Herbalist")]
    public GameObject herbalist;
    public Animator herbalistAnimation;


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
        if (index.y.Equals(currentIndex.y + 1) && coHunterMove == null)
        {
            isForward = true;
            // ������ ����ĥ Ȯ�� ��
            EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape, true);
        }
        EventBus<HuntingEvent>.Publish(HuntingEvent.PlayerMove, isForward, isOnBush);

        if (coHunterMove == null)
        {
            hunterAnimation.SetTrigger("Walk");
            currentIndex = index;
        }
        coHunterMove ??= StartCoroutine(Utility.CoTranslateLookFoward(hunter.transform, hunter.transform.position, pos, 1f, () =>
        {
            hunterAnimation.SetTrigger("Idle");
            hunter.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            coHunterMove = null;
            EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape);
        }));
    }

    public void HuntFailAnimation()
    {
        hunterAnimation.SetTrigger("Fail");
        herbalistAnimation.SetTrigger("Fail");
    }
    public void HuntSuccessAnimation()
    {
        hunterAnimation.SetTrigger("Success");
        herbalistAnimation.SetTrigger("Success");
    }

    // TODO : �� ���� ����� ������ �ٲ� ����
    public void ReturnBow()
    {
        bowBack.SetActive(true);
        bowHand.SetActive(false);
    }


    public void ShootAnimation(Vector3 animalPos)
    {
        hunterAnimation.SetTrigger("ShootingArrow");
        hunter.transform.LookAt(animalPos);
    }
    public void ShootArrow(Vector3 pos)
    {
        shootArrow.gameObject.SetActive(true);
        StartCoroutine(shootArrow.Shoot(pos));
    }
}
