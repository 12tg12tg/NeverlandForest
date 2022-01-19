using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class Animal : MonoBehaviour
{
    public Animator animator;
    public GameObject resultPopUp;

    private int escapePercentUp = 3;
    private int escapePercent;
    public int EscapePercent => escapePercent;

    private void Awake()
    {
        InitEscapingPercentage();
    }


    private void OnEnable()
    {
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.AnimalEscape, EscapingPercentageUp);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.AnimalEscape, Escaping);
    }

    private void OnDestroy()
    {
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.AnimalEscape, EscapingPercentageUp);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.AnimalEscape, Escaping);
    }

    public void Escaping(object[] vals)
    {
        if (vals.Length != 0)
            return;

        // 플레이어가 이동할 때 마다 호출 되어야 하는 메서드
        var rnd = Random.Range(0f, 1f);
        if (rnd < escapePercent * 0.01f)
        {
            Debug.Log($"{rnd} 현재 확률: {escapePercent * 0.01f} 동물 도망 성공");

            resultPopUp.SetActive(true);
            var tm = resultPopUp.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            tm.text = "The Animal Run Away";
            
            StartCoroutine(Utility.CoSceneChange("AS_RandomMap", 3f));
        }
        else
        {
            Debug.Log($"{rnd} 현재 확률: {escapePercent * 0.01f} 동물 도망 실패");
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

        Debug.Log($"현재 도망 확률:{escapePercent}");
    }

    private void InitEscapingPercentage()
    {
        //TODO : 랜턴 + 낮/밤 = 빛 기능 추가시 변경 예정
        var lanternCount = Vars.UserData.uData.LanternCount;
        var step =
            lanternCount < 7 ? 1 :
            lanternCount < 12 ? 2 :
            lanternCount < 16 ? 3 : 4;
        var lanternPercent = step == 1 ? Random.Range(2, 5) : Random.Range(2, 4);

        escapePercent = lanternPercent * step;

        // 도망 확률업은 3 * step
        escapePercentUp *= step;

        Debug.Log($"기본 도망 확률:{escapePercent}");
    }


    public void AnimalMove(bool isDead, UnityAction action)
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
