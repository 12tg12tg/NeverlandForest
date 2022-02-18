using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleGesture : MonoBehaviour
{
    //Instance
    private MultiTouch mt;

    //Rotate Vars
    public Transform target;
    public float RPS = 0f;
    private readonly float maxRPS = 540f;
    public float destRPS;

    public float dv = 0f;
    public float ddv = 0.1f;
    private readonly float minDV = 0.4f;

    private Coroutine accelCo;
    private float delayTimer;
    private bool startDecrease;
    private bool haveToleftDecrease;

    private bool IsLeftTurn { get => RPS > 0; }
    private bool IsRightTurn { get => RPS < 0; }
    //Check
    public bool leftTurnDone;
    public bool rightTurnDone;
    public float prevRotY;

    //Gesture Vars
    private bool fingerIsDown = false;
    private Vector3 touchStart;
    private Vector3 lastDeviationCheck;
    private string touchPattern = string.Empty;
    private string touchPatternChain = string.Empty;
    private float deviationCheckDistance = 1.0f; //üũ�����Ÿ� 5
    private string[] clockCircleChain = new string[2] { "32413", "34132" };
    private string[] counterCircleChain = new string[3] { "42314", "43142", "23142" };

    public CampManager campManager;

    private void Start()
    {
        mt = MultiTouch.Instance;
    }

    private void Update()
    {
        RotateUpdate();
        GestureUpdate();
        CheckUpdate();
    }
    public void Init()
    {
        RPS = 0f;
        prevRotY = 0f;
    }

    private void CheckUpdate()
    {
        var curY = target.rotation.eulerAngles.y;
        if (IsRightTurn)
        {
            bool prev_InLeftHalf = prevRotY < 360f && prevRotY > 340f;
            bool cur_InRightHalf = curY > 0f && curY < 20f;

            if (prev_InLeftHalf && cur_InRightHalf)
            {
                rightTurnDone = true;
            }
        }
        else if (IsLeftTurn)
        {
            bool prev_InRightHalf = prevRotY < 20f && prevRotY > 0f;
            bool cur_InLeftHalf = curY > 340f && curY < 360f;

            if (prev_InRightHalf && cur_InLeftHalf)
            {
                leftTurnDone = true;
            }
        }

        if (rightTurnDone && leftTurnDone)
        {
            /*Ŭ���� ���� �޼�*/
            campManager.CloseRotationPanel();
            rightTurnDone = false;
            leftTurnDone = false;
            campManager.CallMakeCook();
        }
        prevRotY = curY;
    }
    private void GestureUpdate()
    {
        // touch start
        if (!fingerIsDown && mt.IsOneTouchStart)
        {
            fingerIsDown = true;
            touchStart = Camera.main.ScreenToWorldPoint(mt.TouchStart);

            touchStart.z = -1;
            lastDeviationCheck = touchStart;
        }

        // touch end
        if (fingerIsDown && mt.IsOneTouchEnd)
        {
            touchPatternChain = string.Empty;
            touchPattern = string.Empty;
            fingerIsDown = false;
        }

        // touch middle
        if (fingerIsDown && mt.IsOneTouchIng)
        {
            var touchPos = mt.TouchPos;
            var inputPosition = new Vector3(touchPos.x, touchPos.y, 10f);

            Vector3 touchCurrent = Camera.main.ScreenToWorldPoint(inputPosition);
            float diffX = Mathf.Abs(touchCurrent.x - lastDeviationCheck.x);
            float diffY = Mathf.Abs(touchCurrent.y - lastDeviationCheck.y);

            bool deviated = false;

            if (diffX > deviationCheckDistance)
            {
                deviated = true;
                // LEFT -> RIGHT
                if (touchCurrent.x > lastDeviationCheck.x)
                {
                    RecordPattern("1");
                    touchPatternChain += touchPattern;
                }
                // RIGHT -> LEFT
                if (touchCurrent.x < lastDeviationCheck.x)
                {
                    RecordPattern("2");
                    touchPatternChain += touchPattern;
                }
            }

            if (diffY > deviationCheckDistance)
            {
                deviated = true;
                touchPatternChain += touchPattern;
                // TOP -> BOTTOM
                if (touchCurrent.y < lastDeviationCheck.y)
                {
                    RecordPattern("3");
                    touchPatternChain += touchPattern;
                }
                // BOTTOM -> TOP
                if (touchCurrent.y > lastDeviationCheck.y)
                {
                    RecordPattern("4");
                    touchPatternChain += touchPattern;
                }
            }

            if (deviated)
                lastDeviationCheck = touchCurrent;
            else
                Debug.Log("Case : ���� ��ϵ��� ���� ��� �Դϴ�.");

            foreach (var chain in clockCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    Debug.Log($"{chain} in {touchPatternChain} �ð�������� �����ִ�");
                    //Debug.Log("�ٽð�������� �����ִ�");
                    touchPatternChain = string.Empty;
                    touchPattern = string.Empty;
                    RightTurn();
                    break;
                }
            }

            foreach (var chain in counterCircleChain)
            {
                if (touchPatternChain.Contains(chain))
                {
                    Debug.Log($"{chain} in {touchPatternChain} �ݽð�������� �����ִ�");
                    //Debug.Log($"�ڹݽð�������� �����ִ�");
                    touchPatternChain = string.Empty;
                    touchPattern = string.Empty;
                    LeftTurn();
                    break;
                }
            }
        }
    }

    //Gesture
    void RecordPattern(string thisPattern)
    {
        bool isTouchPatternEmpty = touchPattern.Length == 0;
        bool isNewPatternDoesntOverlap = !isTouchPatternEmpty && touchPattern.Substring(touchPattern.Length - 1) != thisPattern;

        if(isTouchPatternEmpty || isNewPatternDoesntOverlap)
            touchPattern += thisPattern;
    }
    private void RotateUpdate()
    {
        var rot = RPS * Time.deltaTime;
        target.transform.rotation *= Quaternion.Euler(0f, -rot, 0f);

        if (!startDecrease)
        {
            DecreaseInit();
        }
        else
        {
            DecreaseSpeed();
        }
    }

    //Rotate
    private void DecreaseInit()
    {
        delayTimer += Time.deltaTime;
        if (delayTimer > 2.5f)
        {
            Debug.Log("���� ����");
            startDecrease = true;
            dv = 1f;

            if (IsLeftTurn)
                haveToleftDecrease = true;
            else
                haveToleftDecrease = false;
        }
    }
    private void DecreaseSpeed()
    {
        if (haveToleftDecrease)
        {
            RPS -= dv;

            if (RPS < 0)
            {
                startDecrease = false;
                Debug.Log("End ����");
                RPS = 0f;
            }
        }
        else
        {
            RPS += dv;

            if (RPS > 0)
            {
                startDecrease = false;
                Debug.Log("End ����");
                RPS = 0f;
            }
        }

        dv -= ddv;
        if (dv < minDV)
            dv = minDV;
    }
    private void LeftTurn()
    {
        startDecrease = false; //�ڵ���� ���� ���.
        delayTimer = 0f;

        if (IsRightTurn && destRPS < 0f)
        {
            //�ݴ� �Է��� ���� ���.
            Debug.Log("�ݴ���� �Է�");
            if (accelCo != null)
                StopCoroutine(accelCo);
            destRPS = 10f;
        }
        else if (accelCo != null) //�̹� �������̶��,
        {
            Debug.Log("�ڷ�ƾ ���� ����");
            StopCoroutine(accelCo);
            destRPS += 60f;
        }
        else //ù�����̶��
        {
            Debug.Log("Start ����");
            destRPS = RPS + 60f;
        }

        if (destRPS > maxRPS)
            destRPS = maxRPS;

        accelCo = StartCoroutine(CoAccelation(RPS, destRPS));
    }
    private void RightTurn()
    {
        startDecrease = false; //�ڵ���� ���� ���.
        delayTimer = 0f;

        if (IsLeftTurn && destRPS > 0f)
        {
            //�ݴ� �Է��� ���� ���.
            Debug.Log("�ݴ���� �Է�");
            if (accelCo != null)
                StopCoroutine(accelCo);
            destRPS = -10f;
        }
        else if (accelCo != null) //�̹� �������̶��,
        {
            Debug.Log("�ڷ�ƾ ���� ����");
            StopCoroutine(accelCo);
            destRPS -= 60f;
        }
        else //ù�����̶��
        {
            Debug.Log("Start ����");
            destRPS = RPS - 60f;
        }
        if (destRPS < -maxRPS)
            destRPS = -maxRPS;
        accelCo = StartCoroutine(CoAccelation(RPS, destRPS));
    }
    IEnumerator CoAccelation(float curSpeed, float destSpeed)
    {
        float timer = 0f;

        while (timer < 1.5f)
        {
            timer += Time.deltaTime;
            var ratio = timer / 1.5f;
            RPS = Mathf.Lerp(curSpeed, destSpeed, ratio);
            yield return null;
        }
        Debug.Log("�ڷ�ƾ ����");
        accelCo = null;
    }



    private void OnGUI()
    {
        if(GUILayout.Button("Left Turn"))
        {
            LeftTurn();
        }

        if (GUILayout.Button("Right Turn"))
        {
            RightTurn();
        }

        if (GUILayout.Button("Restart"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (GUILayout.Button("Cooking"))
        {
            SceneManager.LoadScene("Wt_Scene");
        }
    }
}
