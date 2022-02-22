using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHuntingUnit1 : UnitBase
{
    private Coroutine coMove;
    public GameObject shootArrow;
    public Animator playerAnimation;

    private Vector2 currentIndex;
    public Vector2 CurrentIndex => currentIndex;

    private void Start()
    {
        currentIndex = Vector2.right; // 인덱스 1,0 이라는 의미.. 최초 시작 왼쪽 가운데
    }

    public void Move(Vector2 index, Vector3 pos, bool isOnBush)
    {
        // 플레이어가 이동하면 사냥 할 확률 증가(한칸 앞으로 이동 시 10프로, 은폐물에 숨었을 때 10프로)
        // 동물이 플레이어를 발견 할 확률 증가
        var isForward = false;
        // 현재 위치에서 뒤로 이동, 2칸 앞으로 이동, 같은 칸, 대각선 2칸 이동 막음
        if (index.y <= currentIndex.y - 1 ||
            index.y >= currentIndex.y + 2 ||
            index.Equals(currentIndex) ||
            Mathf.Abs(index.x - currentIndex.x) > 1)
            return;

        // index의 y 값 비교를 통해서 앞으로 한칸 전진 했는지 판단 가능
        if (index.y.Equals(currentIndex.y + 1) && coMove == null)
        {
            isForward = true;
            // 동물이 도망칠 확률 업
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
