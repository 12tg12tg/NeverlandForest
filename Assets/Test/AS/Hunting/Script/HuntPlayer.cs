using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HuntPlayer : UnitBase
{
    [Header("��ɲ�")]
    public Arrow shootArrow;
    public GameObject hunter;
    public Animator hunterAnimation;
    public GameObject bowHand;
    public GameObject bowBack;
    private Coroutine coHunterMove;
    private Vector2 curHunterIndex;
    public Vector2 CurHunterIndex => curHunterIndex;

    [Header("��������")]
    public GameObject herbalist;
    public Animator herbalistAnimation;
    public readonly Vector2 curHerbalistIndex = Vector2.zero;

    [Header("����")]
    private int life = 2;
    public int Life { get => life; set => life = value; }
    public bool IsTutorialClear { get; set; } = false;
    public HuntTile tutorialTile;
    public Vector2 tutorialIndex;

    public void Init()
    {
        // ��� �÷��̾� �ʱ�ȭ�� �ʿ��� �͵� �߰��ؾ� ��
        curHunterIndex = Vector2.right;
    }

    public void Move(Vector2 index, Vector3 pos, bool isOnBush)
    {
        // �÷��̾ �̵��ϸ� ��� �� Ȯ�� ����(��ĭ ������ �̵� �� 10����, ���󹰿� ������ �� 10����)
        // ������ �÷��̾ �߰� �� Ȯ�� ����
        var isForward = false;
        
        // ���� ��ġ���� �ڷ� �̵�, 2ĭ ������ �̵�, ���� ĭ, �밢�� 2ĭ �̵� ����
        if (index.y <= curHunterIndex.y - 1 ||
            index.y >= curHunterIndex.y + 2 ||
            index.Equals(curHunterIndex) ||
            Mathf.Abs(index.x - curHunterIndex.x) > 1)
            return;

        // index�� y �� �񱳸� ���ؼ� ������ ��ĭ ���� �ߴ��� �Ǵ� ����
        if (index.y.Equals(curHunterIndex.y + 1) && coHunterMove == null)
        {
            isForward = true;
            // ������ ����ĥ Ȯ�� ��
            EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscapePercentUp, true);
        }
        EventBus<HuntingEvent>.Publish(HuntingEvent.PlayerMove, isForward, isOnBush);

        if (coHunterMove == null)
        {
            hunterAnimation.SetTrigger("Walk");
            curHunterIndex = index;
        }
        coHunterMove ??= StartCoroutine(Utility.CoTranslateLookFoward(hunter.transform, hunter.transform.position, pos, 1f, () =>
        {
            hunterAnimation.SetTrigger("Idle");
            hunter.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            coHunterMove = null;

            // �÷��̾��� �̵��� ������ ���� ���� üũ
            EventBus<HuntingEvent>.Publish(HuntingEvent.AnimalEscape, this);
        }));
    }

    public void TutorialMove(UnityAction action)
    {
        EventBus<HuntingEvent>.Publish(HuntingEvent.PlayerMove, true, true);

        if (coHunterMove == null)
        {
            hunterAnimation.SetTrigger("Walk");
            curHunterIndex = tutorialTile.index;
        }

        coHunterMove ??= StartCoroutine(Utility.CoTranslateLookFoward(hunter.transform, hunter.transform.position, 
            tutorialTile.transform.position, 1f, () => 
            {
                hunterAnimation.SetTrigger("Idle");
                hunter.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                coHunterMove = null;
                action?.Invoke();
            }));
    }

    public void HuntFailAnimation()
    {
        hunterAnimation.SetTrigger("Fail");
        herbalistAnimation.transform.localRotation = Quaternion.Euler(Vector3.zero);
        herbalistAnimation.SetTrigger("Fail");
    }
    public void HuntSuccessAnimation()
    {
        hunterAnimation.SetTrigger("Success");
        herbalistAnimation.transform.localRotation = Quaternion.Euler(Vector3.zero);
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
