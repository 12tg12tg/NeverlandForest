using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HuntPlayer : UnitBase
{
    [Header("사냥꾼")]
    public Arrow shootArrow;
    public GameObject hunter;
    public Animator hunterAnimation;
    public GameObject bowHand;
    public GameObject bowBack;
    private Coroutine coHunterMove;
    private Vector2 curHunterIndex;
    public Vector2 CurHunterIndex => curHunterIndex;

    [Header("약초학자")]
    public GameObject herbalist;
    public Animator herbalistAnimation;
    public readonly Vector2 curHerbalistIndex = Vector2.zero;

    [Header("공용")]
    private int life = 2;
    public int Life { get => life; set => life = value; }
    public bool IsTutorialClear { get; set; } = false;
    public HuntTile tutorialTile;
    public Vector2 tutorialIndex;

    public void Init()
    {
        // 사냥 플레이어 초기화에 필요한 것들 추가해야 함
        curHunterIndex = Vector2.right;
    }

    public void Move(Vector2 index, Vector3 pos, bool isOnBush)
    {
        // 플레이어가 이동하면 사냥 할 확률 증가(한칸 앞으로 이동 시 10프로, 은폐물에 숨었을 때 10프로)
        // 동물이 플레이어를 발견 할 확률 증가
        var isForward = false;
        
        // 현재 위치에서 뒤로 이동, 2칸 앞으로 이동, 같은 칸, 대각선 2칸 이동 막음
        if (index.y <= curHunterIndex.y - 1 ||
            index.y >= curHunterIndex.y + 2 ||
            index.Equals(curHunterIndex) ||
            Mathf.Abs(index.x - curHunterIndex.x) > 1)
            return;

        // index의 y 값 비교를 통해서 앞으로 한칸 전진 했는지 판단 가능
        if (index.y.Equals(curHunterIndex.y + 1) && coHunterMove == null)
        {
            isForward = true;
            // 동물이 도망칠 확률 업
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

            // 플레이어의 이동이 끝나면 동물 도망 체크
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

    // TODO : 더 좋은 방법이 있으면 바꿀 예정
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
