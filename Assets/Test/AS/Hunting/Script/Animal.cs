using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Animal : MonoBehaviour
{
    public Animator animator;

    private bool isDanger = false;
    public bool isRun = false;
    private readonly int dangerIndex = 3;

    private int escapePercentUp = 3;
    private int escapePercent;
    public int EscapePercent => escapePercent;

    [Header("UI")]
    public Transform target;
    public RectTransform warningBox;
    public Sprite runAway;
    public GameObject failPopUp;
    public GameObject transparentWindow;
    public Button huntButton;
    private Coroutine coFadeOut;
    private readonly float delay = 1f;
    private readonly Vector3 boxOffset = new Vector3(20f, 70f, 0f);

    private void OnEnable()
    {
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.AnimalEscapePercentUp, EscapingPercentageUp);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.AnimalEscape, Escaping);
    }
    private void OnDisable()
    {
        warningBox.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.AnimalEscapePercentUp, EscapingPercentageUp);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.AnimalEscape, Escaping);
    }

    public void Init()
    {
        isDanger = false;

        //TODO : ���� + ��/�� = �� ��� �߰��� ���� ����
        var lanternCount = Vars.UserData.uData.LanternCount;
        var step =
            lanternCount < 7 ? 1 :
            lanternCount < 12 ? 2 :
            lanternCount < 16 ? 3 : 4;
        var lanternPercent = step == 1 ? Random.Range(2, 5) : Random.Range(2, 4);

        escapePercent = lanternPercent * step;
        
        // ���� Ȯ������ 3 * step
        escapePercentUp = 3 * step;

        Debug.Log($"�⺻ ���� Ȯ��:{escapePercent} ���� Ȯ����:{escapePercentUp}");
    }

    private void MoveWarningBox()
    {
        warningBox.gameObject.SetActive(true);
        StartCoroutine(WarningBoxUpdate());

        if (coFadeOut != null)
        {
            StopCoroutine(coFadeOut);
        }
        coFadeOut = StartCoroutine(WarningBoxOff(() => coFadeOut = null));
    }

    private IEnumerator WarningBoxOff(UnityAction action = null)
    {
        var img = warningBox.GetComponent<Image>();
        var temp = img.color = Color.white;
        yield return new WaitForSeconds(delay * 2);

        var timer = 0f;
        while (timer < delay)
        {
            temp.a -= Time.deltaTime;
            img.color = temp;
            timer += Time.deltaTime;
            yield return null;
        }
        warningBox.gameObject.SetActive(false);
        action?.Invoke();
    }

    private IEnumerator WarningBoxUpdate()
    {
        while (warningBox.gameObject.activeSelf)
        {
            var viewPos = Camera.main.WorldToViewportPoint(target.position);
            var canvas = warningBox.transform.parent.GetComponent<RectTransform>().rect;
            viewPos.x *= canvas.width;
            viewPos.y *= canvas.height;
            warningBox.anchoredPosition = viewPos + boxOffset;

            yield return null;
        }
    }

    public void Escaping(object[] vals)
    {
        if (vals.Length != 1)
            return;
        var player = (HuntPlayer)vals[0];

        // �÷��̾ �̵��� �� ���� ȣ�� �Ǿ�� �ϴ� �޼���
        var rnd = Random.Range(0f, 1f);

        var isReducedLife = rnd < escapePercent * 0.01f;
        player.Life = isReducedLife ? player.Life - 1 : player.Life;
        var index = (int)player.CurHunterIndex.y;
        // ù�� ° �÷��̾��� ĭ�� ������ ĭ �����ϰ� �� 5ĭ
        // 3ĭ ������ 2�� �߰��ǰų� 4ĭ ���Ŀ� 1���̶� �߰��Ǹ� ���� ����
        if (player.Life == 0 || (index > dangerIndex && isReducedLife))
        {
            Debug.Log($"{rnd * 100} < ���� Ȯ��: {escapePercent} ���� ���� ����");
            warningBox.GetComponent<Image>().sprite = runAway;
            MoveWarningBox();
            AnimalRunAway();
            player.HuntFailAnimation();
            huntButton.interactable = false;
            AnimalMove(false, () => {
                failPopUp.SetActive(true);
                transparentWindow.SetActive(true);
            });

            StartCoroutine(Utility.CoSceneChange("AS_RandomMap", 3f));
        }
        else if (player.Life.Equals(1) && index <= dangerIndex - 1 && !isDanger)
        {
            isDanger = true; // �ѹ��� �ϱ� ����
            Debug.Log($"{rnd * 100} < ���� Ȯ��: {escapePercent} �������� �߰��� �� �ߴ�");
            MoveWarningBox();
            animator.SetTrigger("Turn");
        }
        else
        {
            Debug.Log($"{rnd * 100} < ���� Ȯ��: {escapePercent} ���� ���� ����");
        }
    }
    public void EscapingPercentageUp(object[] vals)
    {
        if (vals.Length != 1)
            return;

        escapePercent = (bool)vals[0] ? escapePercent + escapePercentUp : escapePercent;

        Debug.Log($"���� ���� Ȯ��:{escapePercent}");
    }

    public void AnimalMove(bool isDead, UnityAction action = null)
    {
        var dest = isDead ? transform.position - new Vector3(0f, 3f, 0f) : transform.position + new Vector3(5f, 0f, 0f);
        StartCoroutine(Utility.CoTranslate(transform, transform.position, dest, 1f, () => {
            gameObject.SetActive(false);
            action?.Invoke();
        }));
    }

    public void AnimalDead()
    {
        animator.SetTrigger("Die");
    }
    public void AnimalRunAway()
    {
        isRun = true;
        animator.SetTrigger("Run");
    }
}
