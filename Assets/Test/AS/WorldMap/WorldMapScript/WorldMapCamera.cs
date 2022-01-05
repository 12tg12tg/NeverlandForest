using UnityEngine;
using System.Collections;

public class WorldMapCamera : MonoBehaviour
{
    public Transform playerPos;

    private Coroutine coCameraMove;
    private Vector3 startPos;
    private float startX;
    private readonly float distance = 30f;
    private readonly float maxDistance = 150f;
    public bool isInit = false;

    public void Init()
    {
        transform.position = new Vector3(playerPos.position.x + 23f, transform.position.y, transform.position.z);
        startPos = transform.position;
        startX = startPos.x - 30f;
        isInit = true; // 씬 전환 시 초기화가 되기전에 동작하지 못하게 막는 용도(간헐적으로 발생하는 카메라 돌아가는 현상 막기 위함)
    }

    private void Update()
    {
        if (coCameraMove != null || !isInit)
            return;

        var touch = GameManager.Manager.MultiTouch;

        if (touch.TouchCount > 0)
        {
            var pos = Camera.main.ScreenToViewportPoint(touch.PrimaryStartPos - touch.PrimaryPos);
            transform.position = new Vector3(pos.x, 0f, 0f) * distance + startPos;
        }
        else
        {
            if (transform.position.x < startX)
            {
                transform.position = new Vector3(startX, transform.position.y, transform.position.z);
            }
            else if (transform.position.x > startX + maxDistance)
            {
                transform.position = new Vector3(startX + maxDistance, transform.position.y, transform.position.z);
            }
            startPos = transform.position;
        }
    }

    public void FollowPlayer()
    {
        if (coCameraMove != null)
            return;

        // 월드맵에서 사용자가 던전맵을 클리어 하면 노드 이동과 함께 실행
        var startPos = new Vector3(playerPos.position.x, transform.position.y, transform.position.z);
        var endPos = Vector3.zero + startPos;
        if (Vars.UserData.WorldMapPlayerData.isClear)
            endPos = new Vector3(10f, 0f, 0f) + startPos;
        
        coCameraMove ??= StartCoroutine(Utility.CoTranslate(transform, startPos, endPos, 1f, () => coCameraMove = null));
    }
}
