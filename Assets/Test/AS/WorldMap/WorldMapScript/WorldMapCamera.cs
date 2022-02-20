using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class WorldMapCamera : MonoBehaviour
{
    public Transform playerPos;

    private float startX;
    private float distance;
    private float maxDistance;

    private Vector3 startPos;
    private Coroutine coCameraMove;

    public static bool isInit = false;
    private bool isStop = false;

    public bool UseMiniMap;

    private MultiTouch multiTouch;

    public void Init()
    {
        if(!UseMiniMap)
        {
            startX = 20f;
            distance = 40f;
            maxDistance = 120f;
            transform.position = new Vector3(playerPos.position.x + startX, transform.position.y, transform.position.z);
        }
        else
        {
            distance = 50f;
            maxDistance = 200f;
            startPos = transform.position;
            startX = startPos.x - distance * 0.1f;
        }

        multiTouch = GameManager.Manager.MultiTouch;
    }

    private void Update()
    {
        if (coCameraMove != null || isStop)
            return;

        if (multiTouch.TouchCount > 0)
        {
            var movePos = multiTouch.PrimaryStartPos - multiTouch.PrimaryPos;
            var posX = Camera.main.ScreenToViewportPoint(movePos).x;
            if (!Mathf.Approximately(Camera.main.rect.width, 1f))
            {
                var reversal = 1f - Camera.main.rect.width;
                var correct = reversal / 2f;
                posX += correct; 
            }
            //Debug.Log($"{Camera.main.rect.width} {Screen.width}");
            //Debug.Log($"{Camera.main.rect.height} {Screen.height}");
            //Debug.Log($"{multiTouch.PrimaryStartPos} {multiTouch.PrimaryPos} {Camera.main.ScreenToViewportPoint(movePos)} {movePos} {posX}");
            if (!Vector3.zero.Equals(startPos) && !Mathf.Approximately(posX, 0f))
            {
                transform.position = new Vector3(posX, 0f, 0f) * distance + startPos;
            }

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

    public void FollowPlayer(UnityAction action = null)
    {
        if (coCameraMove != null)
            return;

        // ����ʿ��� ����ڰ� �������� Ŭ���� �ϸ� ��� �̵��� �Բ� ����
        var startPos = new Vector3(playerPos.position.x - 10f, transform.position.y, transform.position.z);
        var isClear = Vars.UserData.WorldMapPlayerData.isClear;

        var endPos = isClear || !isInit ?
            new Vector3(distance - 5f, 0f, 0f) + startPos :
            Vector3.zero + startPos;

        coCameraMove ??= StartCoroutine(
            Utility.CoTranslate(transform, startPos, endPos, 1f,
            () =>
            {
                coCameraMove = null;
                isInit = true;
                action?.Invoke();
            }));
    }

    public void RunDungoen() => isInit = true;
    public void CameraMoveStop() => isStop = true;
    public void CameraMoveActive() => isStop = false;
}