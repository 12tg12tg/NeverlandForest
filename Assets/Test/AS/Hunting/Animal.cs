using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    public MeshRenderer icon;
    public GameObject resultPopUp;
    public Color colorOrange;

    private int escapePercent;
    public int EscapePercent => escapePercent;

    private void Awake()
    {
        icon.gameObject.transform.position = gameObject.transform.position + new Vector3(0f, 2f, 0f);
        icon.material.color = Color.green;
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.AnimalEscape, Escaping);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.AnimalEscape, EscapingPercentageUp);
    }

    private void OnDestroy()
    {
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.AnimalEscape, Escaping);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.AnimalEscape, EscapingPercentageUp);
    }

    public void Escaping(object[] vals)
    {
        if (vals.Length != 0)
            return;

        // �÷��̾ �̵��� �� ���� ȣ�� �Ǿ�� �ϴ� �޼���
        var rnd = Random.Range(0f, 1f);
        if (rnd < escapePercent * 0.01f)
        {
            Debug.Log($"{rnd} ���� Ȯ��: {escapePercent * 0.01f} ���� ���� ����");

            resultPopUp.SetActive(true);
            var tm = resultPopUp.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            tm.text = "The Animal Run Away";
            
            StartCoroutine(Utility.CoSceneChange("AS_RandomMap", 3f));
        }
        else
        {
            Debug.Log($"{rnd} ���� Ȯ��: {escapePercent * 0.01f} ���� ���� ����");
        }
    }
    public void EscapingPercentageUp(object[] vals)
    {
        escapePercent = vals.Length != 1 || !(bool)vals[0] ? escapePercent + 10 : escapePercent;

        IconColor();
    }

    private void IconColor()
    {
        icon.material.color = 
            escapePercent < 15 ? Color.green :
            escapePercent < 35 ? Color.yellow :
            escapePercent < 55 ? colorOrange : Color.red;
    }
}
