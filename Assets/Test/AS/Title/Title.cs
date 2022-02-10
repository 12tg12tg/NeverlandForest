using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Title : MonoBehaviour
{
    private Coroutine coTapToStart;
    private bool isStart = false;

    public TextMeshProUGUI startText;
    public Image panel;
    public Image fadeOut;

    private void Update()
    {
        coTapToStart ??= StartCoroutine(CoStartTextFadeIn(() => {
            coTapToStart = null;
            isStart = true;
        }));

        if (isStart && GameManager.Manager.MultiTouch.TouchCount > 0)
        {
            var manager = GameManager.Manager;
            StartCoroutine(CoFadeOut(() => {
                gameObject.SetActive(false);
                manager.TutoManager.Init();
                //manager.Production.black.SetActive(true); // TODO : 잠시 주석
            }));
        }
    }

    private IEnumerator CoStartTextFadeIn(UnityAction action)
    {
        var white = new Color(1, 1, 1, 0);
        var black = Color.clear;

        var time = 1f;
        var timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;
            var value = Mathf.Lerp(0, 1, ratio);
            startText.color = white;
            panel.color = black;
            white.a = value;
            black.a = value / 2;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }

    private IEnumerator CoFadeOut(UnityAction action)
    {
        var black = Color.clear;

        var time = 1f;
        var timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;
            var value = Mathf.Lerp(0, 1, ratio);
            black.a = value;
            fadeOut.color = black;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }
}
