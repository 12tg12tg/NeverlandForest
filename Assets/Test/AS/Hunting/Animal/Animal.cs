using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class Animal : MonoBehaviour
{
    public Animator animator;
    public GameObject resultPopUp;

    private bool isDanger = false;
    private int escapePercentUp = 3;
    private int escapePercent;
    public int EscapePercent => escapePercent;

    //private void Awake()
    //{
    //    InitEscapingPercentage();
    //}


    private void OnEnable()
    {
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.AnimalEscapePercentUp, EscapingPercentageUp);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.AnimalEscape, Escaping);
    }

    private void OnDestroy()
    {
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.AnimalEscapePercentUp, EscapingPercentageUp);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.AnimalEscape, Escaping);
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
        
        // 3ĭ ������ 2�� �߰��ǰų� 4ĭ ���Ŀ� 1���̶� �߰��Ǹ� ���� ����
        if (player.Life == 0 || (player.CurrentIndex.y > 2 && isReducedLife))
        {
            Debug.Log($"{rnd * 100} < ���� Ȯ��: {escapePercent} ���� ���� ����");
            AnimalRunAway();
            AnimalMove(false);
            resultPopUp.SetActive(true);
            var tm = resultPopUp.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            tm.text = "The Animal Run Away";

            StartCoroutine(Utility.CoSceneChange("AS_RandomMap", 3f));
        }
        else if (player.Life.Equals(1) && player.CurrentIndex.y < 3 && !isDanger)
        {
            isDanger = true; // �ѹ��� �ϱ� ����
            Debug.Log($"{rnd * 100} < ���� Ȯ��: {escapePercent} �������� �߰��� �� �ߴ�");
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
        GetComponent<AnimalStateIcon>().IconColor(escapePercent);
        if(escapePercent >= 20)
            animator.SetTrigger("Turn");

        Debug.Log($"���� ���� Ȯ��:{escapePercent}");
    }

    public void InitEscapingPercentage()
    {
        //TODO : ���� + ��/�� = �� ��� �߰��� ���� ����
        var lanternCount = Vars.UserData.uData.LanternCount;
        var step =
            lanternCount < 7 ? 1 :
            lanternCount < 12 ? 2 :
            lanternCount < 16 ? 3 : 4;
        var lanternPercent = step == 1 ? Random.Range(2, 5) : Random.Range(2, 4);

        escapePercent = lanternPercent * step;

        // ���� Ȯ������ 3 * step
        escapePercentUp *= step;

        Debug.Log($"�⺻ ���� Ȯ��:{escapePercent}");
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
        animator.SetTrigger("Run");
    }

}
