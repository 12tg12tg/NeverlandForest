using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class BattleMessage : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void PrintMessage(string message, float time, UnityAction action)
    {
        gameObject.SetActive(true);
        StartCoroutine(CoMessagePrintOut(message, time, action));
    }

    private IEnumerator CoMessagePrintOut(string message, float time, UnityAction action)
    {
        text.text = message;
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
        action.Invoke();
    }
}
