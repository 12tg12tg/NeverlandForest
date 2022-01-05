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
        isInit = true; // �� ��ȯ �� �ʱ�ȭ�� �Ǳ����� �������� ���ϰ� ���� �뵵(���������� �߻��ϴ� ī�޶� ���ư��� ���� ���� ����)
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

        // ����ʿ��� ����ڰ� �������� Ŭ���� �ϸ� ��� �̵��� �Բ� ����
        var startPos = new Vector3(playerPos.position.x, transform.position.y, transform.position.z);
        var endPos = Vector3.zero + startPos;
        if (Vars.UserData.WorldMapPlayerData.isClear)
            endPos = new Vector3(10f, 0f, 0f) + startPos;
        
        coCameraMove ??= StartCoroutine(Utility.CoTranslate(transform, startPos, endPos, 1f, () => coCameraMove = null));
    }
}
