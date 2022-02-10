using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;

public class TutorialPlayerMove : MonoBehaviour
{
    private Animator playerAnimationBoy;
    private Animator playerAnimationGirl;

    private MultiTouch multiTouch;
    private float speed = 10f;

    public bool isCoMove;

    public PlayerDungeonUnit playerBoy;
    public PlayerDungeonUnit playerGirl;

    // 달리기 움직임 및 손잡기 관련
    public float boySpeed;
    private bool isRunReady;
    private bool isRun;
    private bool isHand;
    private bool isTurn;

    private RigBuilder rigBuilder;
    private Rig boyRig;
    private Rig[] girlRigs;
    private IKControl boyRight;
    private List<RigLayer> girlRiglayers;

    private Coroutine coHand;

    private float tutorialTime;

    private MoveTutorial moveTutorial;
    private GatheringTutorial gatheringTutorial;
    private MainRoomTutorial mainRoomTutorial;

    public void Init()
    {
        multiTouch = GameManager.Manager.MultiTouch;
        playerAnimationBoy = playerBoy.GetComponent<Animator>();
        playerAnimationGirl = playerGirl.GetComponent<Animator>();

        rigBuilder = playerGirl.GetComponent<RigBuilder>();
        girlRiglayers = rigBuilder.layers;

        foreach (var obj in girlRiglayers)
            obj.active = false;

        boyRig = playerBoy.GetComponentInChildren<Rig>();
        girlRigs = playerGirl.GetComponentsInChildren<Rig>();
        boyRight = playerBoy.GetComponent<IKControl>();

        // 0 active = 오른쪽 방향
        // 1 active = 왼쪽방향 이동기준
        girlRiglayers[1].active = true;
        boyRight.isRight = false;
    }


    void Update()
    {
        var dungeonSystem = DungeonSystem.Instance;
        moveTutorial = dungeonSystem.moveTutorial;
        gatheringTutorial = dungeonSystem.gatherTutorial;
        mainRoomTutorial = dungeonSystem.mainRoomTutorial;
        if(mainRoomTutorial.isMainRoomTutorial)
        {
            playerAnimationBoy.SetFloat("Speed", 0);
            playerAnimationGirl.SetFloat("Speed", 0);
            RigOff();
            return;
        }

        if (gatheringTutorial.isGatheringTutorial/* && gatheringTutorial.TutorialStep != 7*/)
        {
            RigOff();
            return;
        }

        if (moveTutorial.isMoveTutorial && moveTutorial.TutorialStep != 2)
        {
            RigOff();
            playerAnimationBoy.SetFloat("Speed", 0);
            playerAnimationGirl.SetFloat("Speed", 0);
            return;
        }
        if (multiTouch != null)
        {
            if (!isCoMove)
            {
                if (multiTouch.TouchCount > 0 /*&& isRayCol*/)
                {
                    boySpeed = Input.GetAxis("MouseAxes");
                    boySpeed *= speed;
                    if (EventSystem.current.IsPointerOverGameObject())
                        return;
                    // 내가 터치하고 있을 때 플레이어보다 왼쪽인지 오른쪽인지 판단하는 형태로 구현하기..
                    var touchXPos = Camera.main.ScreenToViewportPoint(multiTouch.PrimaryPos).x;
                    var playerXPos = Camera.main.WorldToViewportPoint(playerBoy.transform.localPosition).x; // 보이가 기준
                    if (playerXPos + 0.05f < touchXPos)
                    {
                        if (moveTutorial != null && moveTutorial.TutorialStep == 2 &&
                            playerBoy.transform.position.x >= DungeonSystem.Instance.roomGenerate.roomList[0].endPosVector.x)
                            return;

                        if (moveTutorial != null && moveTutorial.TutorialStep == 2)
                        {
                            if (moveTutorial.CommandSucess == 0)
                            {
                                tutorialTime += Time.deltaTime;

                                if (tutorialTime > 1.0f)
                                {
                                    moveTutorial.CommandSucess++;
                                    tutorialTime = 0f;
                                }
                            }
                        }

                        if (!isTurn)
                            RigOff();
                        isTurn = true;

                        var pos = boySpeed * Time.deltaTime * Vector3.right;
                        playerBoy.transform.position += pos;
                        playerBoy.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));

                    }
                    else if (playerXPos - 0.05f > touchXPos)
                    {
                        if (playerBoy.transform.position.x <= DungeonSystem.Instance.roomGenerate.spawnPos.x)
                            return;

                        if (moveTutorial != null && moveTutorial.TutorialStep == 2)
                        {
                            if (moveTutorial.CommandSucess == 1)
                            {
                                tutorialTime += Time.deltaTime;

                                if (tutorialTime > 1.0f)
                                {
                                    moveTutorial.CommandSucess++;
                                    tutorialTime = 0f;
                                    moveTutorial.TutorialStep++;
                                    moveTutorial.delay = 0f;
                                }
                            }
                        }

                        if (isTurn)
                            RigOff();
                        isTurn = false;

                        var pos = boySpeed * Time.deltaTime * -Vector3.right;
                        playerBoy.transform.position += pos;
                        playerBoy.transform.rotation = Quaternion.Euler(new Vector3(0f, 270, 0f));
                    }
                    else
                        boySpeed = 0f;

                    // Girl Move
                    if (playerBoy.transform.position.x > playerGirl.transform.position.x + 1.15f)
                    {
                        girlRiglayers[1].active = false;
                        girlRiglayers[0].active = true;
                        boyRight.isRight = true;
                        var pos = boySpeed * Time.deltaTime * Vector3.right;
                        playerGirl.transform.position += pos;
                        playerGirl.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));

                        if (isHand)
                        {
                            if (coHand != null)
                            {
                                StopCoroutine(coHand);
                                coHand = null;
                                RigOff();
                            }
                            coHand ??= StartCoroutine(HandShakeOn());
                            isHand = false;
                        }
                    }
                    else if (playerBoy.transform.position.x < playerGirl.transform.position.x - 1.15f)
                    {
                        girlRiglayers[1].active = true;
                        girlRiglayers[0].active = false;
                        boyRight.isRight = false;
                        var pos = boySpeed * Time.deltaTime * -Vector3.right;
                        playerGirl.transform.position += pos;
                        playerGirl.transform.rotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));

                        if (isHand)
                        {
                            if (coHand != null)
                            {
                                StopCoroutine(coHand);
                                coHand = null;
                                RigOff();
                            }
                            coHand ??= StartCoroutine(HandShakeOn());
                            isHand = false;
                        }
                    }
                    // 캐릭터 둘이 거리가 가까워질때
                    else
                    {
                        if (!isHand)
                        {
                            if (coHand != null)
                            {
                                StopCoroutine(coHand);
                                coHand = null;
                            }
                            coHand ??= StartCoroutine(HandShakeOff());
                            isHand = true;
                        }
                    }
                }
                // 터치 안누를때
                else
                {
                    boySpeed = 0f;
                    if (!isHand)
                    {
                        if (coHand != null)
                        {
                            StopCoroutine(coHand);
                            coHand = null;
                        }
                        coHand ??= StartCoroutine(HandShakeOff());
                        isHand = true;
                    }
                }

                playerAnimationBoy.SetFloat("Speed", boySpeed);
                playerAnimationGirl.SetFloat("Speed", boySpeed);
            }
            else
            {
                RigOff();
                isHand = true;
            }
        }
    }



    private IEnumerator HandShakeOn()
    {
        RigOn();
        var timer = 0f;
        var handSpeed = 10f;
        while (timer < 1.1f)
        {
            boyRig.weight = Mathf.Lerp(0f, 1f, timer);
            if (girlRiglayers[0].active == true)
                girlRigs[0].weight = boyRig.weight;
            else
                girlRigs[1].weight = boyRig.weight;
            boyRight.weightf = boyRig.weight;
            timer += Time.deltaTime * handSpeed;
            yield return null;
        }
        coHand = null;
    }
    private IEnumerator HandShakeOff()
    {
        var timer = 0f;
        var handSpeed = 3f;
        while (timer < 1.1f)
        {
            boyRig.weight = Mathf.Lerp(1f, 0f, timer);
            if (girlRiglayers[0].active == true)
                girlRigs[0].weight = boyRig.weight;
            else
                girlRigs[1].weight = boyRig.weight;
            boyRight.weightf = boyRig.weight;
            timer += Time.deltaTime * handSpeed;
            yield return null;
        }
        coHand = null;
        RigOff();
    }
    private void RigOff()
    {
        boyRight.ikActive = false;
        boyRig.enabled = false;
        foreach (var obj in girlRiglayers)
            obj.active = false;
        girlRigs[0].weight = 0f;
        girlRigs[1].weight = 0f;
        boyRig.weight = 0f;
    }
    private void RigOn()
    {
        boyRight.ikActive = true;
        boyRig.enabled = true;
        foreach (var obj in girlRiglayers)
            obj.active = true;
    }

    public void AnimationChange()
    {
        playerAnimationBoy.SetFloat("Speed", boySpeed);
    }
}

// 임시
//if (MultiTouch.Instance.IsTap)
//{
//    moveSpeed *= 5f;
//}
//else
//{
//    //boyRig.weight = Mathf.Lerp(0f, 1f, moveSpeed);
//    //girlRig.weight = boyRig.weight;
//    //boyRight.weightf = boyRig.weight;
//    moveSpeed *= 5f;
//}