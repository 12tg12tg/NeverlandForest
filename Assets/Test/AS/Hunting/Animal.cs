using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Events;

public class Animal : MonoBehaviour
{
    public MeshRenderer icon;
    public Animator animator;
    public GameObject resultPopUp;
    public Color colorOrange;

    private int escapePercent;
    public int EscapePercent => escapePercent;

    private void Awake()
    {
        icon.gameObject.transform.position = gameObject.transform.position + new Vector3(0f, 2f, 0f);
        icon.material.color = Color.green;
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
        escapePercent = vals.Length != 1 || !(bool)vals[0] ? escapePercent + 10 : escapePercent;
        IconColor();
        if(escapePercent >= 20)
            animator.SetTrigger("Turn");
    }

    private void IconColor()
    {
        icon.material.color = 
            escapePercent < 15 ? Color.green :
            escapePercent < 35 ? Color.yellow :
            escapePercent < 55 ? colorOrange : Color.red;
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            other.gameObject.SetActive(false);
            EventBus<HuntingEvent>.Publish(HuntingEvent.Hunting);
        }
    }
}
