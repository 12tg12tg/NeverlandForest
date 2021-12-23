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
        text.text = "옛날 어느 울창한 숲속 마을에 한 약초학자가 있었습니다.";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        text.color = Color.white;
        text.text = "그녀는 마을에서 알아주던 실력자였어요.";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        text.color = Color.white;
        text.text = "그러던 어느날 (마녀가 된 계기) ... ";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        text.color = Color.white;
        text.text = "그녀가 왜 영생의 물약에 손을 댔는진 그 누구도 알 수 없어요.";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        text.color = Color.white;
        text.text = "지금부터 펼쳐질 이야기는 마녀로부터 도망쳐 나온 어린 아이,\n ";

        yield return new WaitForSeconds(1.5f);
        text.text = "지금부터 펼쳐질 이야기는 마녀로부터 도망쳐 나온 어린 아이,\n" +
            "어쩌면 당신의 이야기이기도 해요.";

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(CoFadeText());

        Debug.Log("End Coroutine");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("JYK_Test_Main");
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
