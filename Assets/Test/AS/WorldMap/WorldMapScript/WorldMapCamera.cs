using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class WorldMapCamera : MonoBehaviour
{
    public Transform playerPos;

    private Coroutine coCameraMove;
    private Vector3 startPos;
    private readonly float startX = 0f;
    private readonly float distance = 30f;
    private readonly float maxDistance = 120f;
    public bool isInit = false;

    public void Awake()
    {
        transform.position = new Vector3(playerPos.position.x + startX, transform.position.y, transform.position.z);
    }

    private void Update()
    {
        if (coCameraMove != null)
            return;

        var touch = GameManager.Manager.MultiTouch;

        if (touch.TouchCount > 0)
        {
            var pos = Camera.main.ScreenToViewportPoint(touch.PrimaryStartPos - touch.PrimaryPos);
            if (!Vector3.zero.Equals(startPos))
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
        var startPos = new Vector3(playerPos.position.x - 10f, transform.position.y, transform.position.z);
        var endPos = Vars.UserData.WorldMapPlayerData.isClear ? new Vector3(10f, 0f, 0f) + startPos : Vector3.zero + startPos;
        coCameraMove ??= StartCoroutine(Utility.CoTranslate(transform, startPos, endPos, 1f, () => coCameraMove = null));
    }
}