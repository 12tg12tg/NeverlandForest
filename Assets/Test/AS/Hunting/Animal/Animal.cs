using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class Animal : MonoBehaviour
{
    public Animator animator;
    public GameObject resultPopUp;

    public int dangerIndex = 3;
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
        
        // 플레이어가 이동할 때 마다 호출 되어야 하는 메서드
        var rnd = Random.Range(0f, 1f);

        var isReducedLife = rnd < escapePercent * 0.01f;
        player.Life = isReducedLife ? player.Life - 1 : player.Life;

        // 첫번 째 플레이어의 칸과 동물의 칸 제외하고 총 5칸
        // 3칸 내에서 2번 발각되거나 4칸 이후에 1번이라도 발각되면 동물 도망
        if (player.Life == 0 || (player.CurrentIndex.y > dangerIndex && isReducedLife))
        {
            Debug.Log($"{rnd * 100} < 현재 확률: {escapePercent} 동물 도망 성공");
            AnimalRunAway();
            AnimalMove(false);
            resultPopUp.SetActive(true);
            var tm = resultPopUp.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            tm.text = "The Animal Run Away";

            StartCoroutine(Utility.CoSceneChange("AS_RandomMap", 3f));
        }
        else if (player.Life.Equals(1) && player.CurrentIndex.y < dangerIndex - 1 && !isDanger)
        {
            isDanger = true; // 한번만 하기 위함
            Debug.Log($"{rnd * 100} < 현재 확률: {escapePercent} 동물에게 발각될 뻔 했다");
            animator.SetTrigger("Turn");
        }
        else
        {
            Debug.Log($"{rnd * 100} < 현재 확률: {escapePercent} 동물 도망 실패");
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

    public void InitEscapingPercentage()
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
