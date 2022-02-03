using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Title : MonoBehaviour
{
    private Coroutine coTapToStart;
    public TextMeshProUGUI startText;
    public GameObject panel;

    private void Start()
    {
        coTapToStart ??= StartCoroutine(CoAlphaMax(() => coTapToStart = null));
    }
    private void Update()
    {
        if(coTapToStart == null && GameManager.Manager.MultiTouch.TouchCount > 0)
        {
            gameObject.SetActive(false);
        }
    }
    public IEnumerator CoAlphaMax(UnityAction action)
    {
        var newColor = new Color(1, 1, 1, 0);
        var value = 1 / 255f;
        while (newColor.a < 1)
        {
            newColor.a += value;
            startText.color = newColor;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);
        panel.SetActive(true);
        action?.Invoke();
    }
}
