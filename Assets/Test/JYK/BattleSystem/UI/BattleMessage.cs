using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class BattleMessage : MonoBehaviour
{
    public CanvasGroup cg;
    public TextMeshProUGUI text;

    public void PrintMessage(string message, float time, UnityAction action)
    {
        gameObject.SetActive(true);

        StartCoroutine(CoNormalMessagePrintOut(message, time, action));
    }

    public void PrintMessageFadeOut(string message, float time, float fadeTime, UnityAction action)
    {
        gameObject.SetActive(true);
        cg.alpha = 1f;
        StartCoroutine(CoFadeMessagePrintOut(message, time, fadeTime, action));
    }

    private IEnumerator CoNormalMessagePrintOut(string message, float time, UnityAction action)
    {
        text.text = message;
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
        action?.Invoke();
    }

    private IEnumerator CoFadeMessagePrintOut(string message, float time, float fadeTime, UnityAction action)
    {
        text.text = message;
        yield return new WaitForSeconds(time);

        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            var ratio = timer / fadeTime;
            var lerp = Mathf.Lerp(1f, 0f, ratio);
            cg.alpha = lerp;
            yield return null;
        }

        gameObject.SetActive(false);
        action?.Invoke();
    }
}
