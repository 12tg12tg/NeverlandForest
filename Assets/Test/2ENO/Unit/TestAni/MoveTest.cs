using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;

public class MoveTest : MonoBehaviour
{
    private Animator playerAnimationBoy;
    private Animator playerAnimationGirl;
    
    private PlayerMoveAnimation curAnimation;
    private MultiTouch multiTouch;
    private float speed = 10f;

    public bool isCoMove;

    public GameObject playerBoy;
    public GameObject playerGirl;

    // �޸��� ������ �� ����� ����
    public float boySpeed;
    private float girlSpeed;
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
    //void Start()
    //{
    //    multiTouch = GameManager.Manager.MultiTouch;
    //    playerAnimationBoy = playerBoy.GetComponent<Animator>();
    //    playerAnimationGirl = playerGirl.GetComponent<Animator>();

    //    rigBuilder = playerGirl.GetComponent<RigBuilder>();
    //    girlRiglayers = rigBuilder.layers;

    //    foreach (var obj in girlRiglayers)
    //        obj.active = false;

    //    boyRig = playerBoy.GetComponentInChildren<Rig>();
    //    girlRigs = playerGirl.GetComponentsInChildren<Rig>();
    //    boyRight = playerBoy.GetComponent<IKControl>();

    //    // 0 active = ������ ����
    //    // 1 active = ���ʹ��� �̵�����
    //    girlRiglayers[1].active = true;
    //    boyRight.isRight = false;
    //}

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

        // 0 active = ������ ����
        // 1 active = ���ʹ��� �̵�����
        girlRiglayers[1].active = true;
        boyRight.isRight = false;
    }

    // �ӽ�

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


    void Update()
    {
        // ����ĳ��Ʈ �ʿ�
        // UI�� �ƴ� �� �����ϰԲ� ��������
        // ù ��ġ �������θ� ����

        var isRayCol = Physics.Raycast(Camera.main.ScreenPointToRay(multiTouch.PrimaryStartPos), out _, Mathf.Infinity);
        if (!isCoMove)
        {
            if (multiTouch.TouchCount > 0 /*&& isRayCol*/)
            {
                boySpeed = Input.GetAxis("MouseAxes");
                boySpeed *= speed;
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                // ���� ��ġ�ϰ� ���� �� �÷��̾�� �������� ���������� �Ǵ��ϴ� ���·� �����ϱ�..
                var touchXPos = Camera.main.ScreenToViewportPoint(multiTouch.PrimaryPos).x;
                var playerXPos = Camera.main.WorldToViewportPoint(playerBoy.transform.localPosition).x; // ���̰� ����
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
                // ĳ���� ���� �Ÿ��� ���������
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
            // ��ġ �ȴ�����
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

            playerAnimationBoy.SetFloat("Speed", 10);
            playerAnimationGirl.SetFloat("Speed", 10);
        }
        else
        {
            RigOff();
            isHand = true;
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
        //if (isMove)
        //    PlayWalkAnimation();
        //else
        //    PlayIdleAnimation();
        playerAnimationBoy.SetFloat("Speed", boySpeed);
    }

    private void PlayWalkAnimation()
    {
        if (curAnimation == PlayerMoveAnimation.Walk)
            return;
        curAnimation = PlayerMoveAnimation.Walk;
        playerAnimationBoy.SetTrigger("Walk");
        playerAnimationGirl.SetTrigger("Walk");
    }

    private void PlayIdleAnimation()
    {
        if (curAnimation == PlayerMoveAnimation.Idle)
            return;
        curAnimation = PlayerMoveAnimation.Idle;
        playerAnimationBoy.SetTrigger("Idle");
        playerAnimationGirl.SetTrigger("Idle");
    }
}
