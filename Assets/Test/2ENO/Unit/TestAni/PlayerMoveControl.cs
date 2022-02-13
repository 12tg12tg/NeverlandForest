using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;

public class PlayerMoveControl : MonoBehaviour
{
    private Animator playerAnimationBoy;
    private Animator playerAnimationGirl;
    
    private MultiTouch multiTouch;
    private float speed = 10f;

    public bool isCoMove;

    public PlayerDungeonUnit playerBoy;
    public PlayerDungeonUnit playerGirl;

    public GameObject lanternRight;
    public GameObject lanternLeft;

    //private Transform lanternRightRun;
    //private Transform lanternLeftRun;

    [Header("팝업창들")]
    public GameObject dungeonMinimap;
    public GameObject wolrdMinimap;
    public GameObject randomEvent;
    public GameObject huntingPopup;

    private Vector3 lanternIdle;

    private Vector3 lanternRightRunPos;
    private Vector3 lanternRightRunRotate;
    private Vector3 lanternLeftRunPos;
    private Vector3 lanternLeftRunRotate;

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
    public void Init()
    {
        lanternIdle = new Vector3(-1.46f, -176.92f, 129f);
        //lanternRightRunPos = new Vector3(-0.05f, 0.008f, -0.04f);
        lanternRightRunRotate = new Vector3(54.3f, -285.5f, 85.72f);

        //lanternLeftRunPos = new Vector3(-0.06f, 0.025f, 0.005f);
        lanternLeftRunRotate = new Vector3(-41.5f, -62.77f, 53.22f);

        multiTouch = GameManager.Manager.MultiTouch;
        lanternRight.SetActive(true);
        lanternLeft.SetActive(false);

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
        if(dungeonMinimap.activeSelf || wolrdMinimap.activeSelf
            || randomEvent.activeSelf || huntingPopup.activeSelf)
        {
            RigOff();
            playerAnimationBoy.SetFloat("Speed", 0f);
            playerAnimationGirl.SetFloat("Speed", 0f);
            return;
        }

        //var isRayCol = Physics.Raycast(Camera.main.ScreenPointToRay(multiTouch.PrimaryStartPos), out _, Mathf.Infinity);
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
                        lanternRight.SetActive(true);
                        lanternLeft.SetActive(false);

                        lanternRight.transform.localRotation = Quaternion.Euler(lanternRightRunRotate);

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
                        lanternRight.SetActive(false);
                        lanternLeft.SetActive(true);

                        lanternLeft.transform.localRotation = Quaternion.Euler(lanternLeftRunRotate);

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
                        lanternLeft.transform.localRotation = Quaternion.Euler(lanternIdle);
                        lanternRight.transform.localRotation = Quaternion.Euler(lanternIdle);
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
                    lanternLeft.transform.localRotation = Quaternion.Euler(lanternIdle);
                    lanternRight.transform.localRotation = Quaternion.Euler(lanternIdle);
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
                if (boySpeed>0f)
                {
                    //SoundManager.Instance.PlayWalkSound(true);
                }
                else
                {
                    //SoundManager.Instance.PlayWalkSound(false);
                }

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
        while(timer < 1.1f)
        {
            boyRig.weight = Mathf.Lerp(0f, 1f, timer);
            if(girlRiglayers[0].active == true)
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
