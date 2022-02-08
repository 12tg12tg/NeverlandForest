using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public TextMeshProUGUI text;
    void Start()
    {
        StartCoroutine(CoStartStory());
    }

    IEnumerator CoStartStory()
    {
        yield return new WaitForSeconds(1f);

        text.enabled = true;
        text.color = Color.white;
        text.text = "���� ��� ��â�� ���� ������ �� �������ڰ� �־����ϴ�.";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        text.color = Color.white;
        text.text = "�׳�� �������� �˾��ִ� �Ƿ��ڿ����.";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        text.color = Color.white;
        text.text = "�׷��� ����� (���డ �� ���) ... ";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        text.color = Color.white;
        text.text = "�׳డ �� ������ ���࿡ ���� ����� �� ������ �� �� �����.";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        text.color = Color.white;
        text.text = "���ݺ��� ������ �̾߱�� ����κ��� ������ ���� � ����,\n ";

        yield return new WaitForSeconds(1.5f);
        text.text = "���ݺ��� ������ �̾߱�� ����κ��� ������ ���� � ����,\n" +
            "��¼�� ����� �̾߱��̱⵵ �ؿ�.";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        Debug.Log("End Coroutine");
        yield return new WaitForSeconds(1.5f);
        GameManager.Manager.LoadScene(GameScene.Camp);
    }

    IEnumerator CoFadeText()
    {
        float time = 1f;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;
            var lerp = Mathf.Lerp(1, 0, ratio);
            var curColor = text.color;
            curColor.a = lerp;
            text.color = curColor;
            yield return null;
        }
    }
}
