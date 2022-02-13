using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;

public enum StoryType
{
    Prologue,
    Chapter1,
    Chapter2,
    Chapter3,
    Chapter4,
    Chapter5,
    Ending,
}

public enum StoryChar
{
    Narration,
    Hunter,
    Monologue,
    Herbalist
}


public class StoryManager : MonoBehaviour
{
    [Header("대화창")]
    public GameObject messageBox;
    public TMP_Text talker;
    public TMP_Text talk;
    public GameObject hunter;
    public GameObject herbalist;
    public GameObject MessageBox => messageBox;


    [Header("화살표 UI")]
    [SerializeField] private RectTransform target;
    [SerializeField] private float distance;

    private Coroutine CoFade;
    private Vector2 startPos;

    public Image fadeInOut;

    public bool isNext;

    public bool isGameReset = false;

    private void Awake()
    {
        startPos = target.anchoredPosition;
    }

    private void Update()
    {
        if (messageBox.activeSelf)
        {
            CoFade ??= StartCoroutine(UpDownLoop(distance, () => CoFade = null));
        }
    }

    public void EndingStory(TMP_Text text, UnityAction action = null) => StartCoroutine(CoEnding(text, action));

    private IEnumerator UpDownLoop(float posY, UnityAction action)
    {
        var endPos = startPos + new Vector2(0f, posY);
        var time = 1f;
        var timer = 0f;
        while (timer < time)
        {
            var ratio = timer / time;
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, ratio);
            timer += Time.deltaTime;
            yield return null;
        }
        action?.Invoke();
    }

    public IEnumerator CoStory(StoryType storyType, UnityAction action = null)
    {
        var storyTable = DataTableManager.GetTable<TutorialStoryDataTable>().data;
        var datas = storyTable.Where(x => (x.Value as TutorialStoryDataTableElem).index == (int)storyType)
                              .Select(x => x)
                              .ToList();
        talk.text = "";
        for (int i = 0; i < datas.Count; i++)
        {
            var data = (TutorialStoryDataTableElem)datas[i].Value;

            var description = data.description;
            var colorData = data.color;
            var typing = data.typing;
            var character = data.character;

            if(character == StoryChar.Hunter.ToString())
            {
                talker.text = "사냥꾼";
                ColorUtility.TryParseHtmlString("#fffa7c", out var color);
                talker.color = color;
                hunter.SetActive(true);
                herbalist.SetActive(false);
            }
            else if(character == StoryChar.Herbalist.ToString())
            {
                talker.text = "약초학자";
                ColorUtility.TryParseHtmlString("#ff9000", out var color);
                talker.color = color;
                herbalist.SetActive(true);
                hunter.SetActive(false);
            }
            else
            {
                herbalist.SetActive(false);
                hunter.SetActive(false);
                talker.text = " ";
            }

            if (colorData.Length > 0)
            {
                ColorUtility.TryParseHtmlString(colorData, out var color);
                talk.color = color;
            }

            for (int j = 0; j < description.Length; j++)
            {
                if (description[j].Equals('n'))
                {
                    talk.text += "\n";
                }
                else
                    talk.text += description[j];
                if (typing)
                    yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitWhile(() => !isNext);

            talk.text = "";
            talk.color = Color.white;
            isNext = false;
        }
        herbalist.SetActive(false);
        hunter.SetActive(false);
        StartCoroutine(CoFadeOut(() => action?.Invoke()));
    }

    public IEnumerator CoEnding(TMP_Text text, UnityAction action = null)
    {
        var storyTable = DataTableManager.GetTable<TutorialStoryDataTable>().data;
        var datas = storyTable.Where(x => (x.Value as TutorialStoryDataTableElem).index == (int)StoryType.Ending)
                              .Select(x => x)
                              .ToList();
        for (int i = 0; i < datas.Count; i++)
        {
            text.text = "";
            text.color = Color.white;

            var data = (TutorialStoryDataTableElem)datas[i].Value;
            var description = data.description;
            var colorData = data.color;
            var option = data.option;
            var typing = data.typing;
            text.fontSize = typing ? 48 : 36;
            if (colorData.Length > 0)
            {
                ColorUtility.TryParseHtmlString(colorData, out var color);
                text.color = color;
            }
            text.text += option ? @"""" : "";

            for (int j = 0; j < description.Length; j++)
            {
                if (description[j].Equals('n'))
                {
                    text.text += "\n";
                }
                else
                    text.text += description[j];
                if (typing)
                    yield return new WaitForSeconds(0.1f);
            }

            text.text += option ? @"""" : "";

            yield return new WaitForSeconds(1f);

            if(i != datas.Count -1)
            {
                float time = 1f;
                float timer = 0f;

                while (timer < time)
                {
                    timer += Time.deltaTime;
                    var ratio = timer / time;
                    var curColor = text.color;
                    curColor.a = Mathf.Lerp(1, 0, ratio);
                    text.color = curColor;
                    yield return null;
                }
            }
        }

        action?.Invoke();
        yield return new WaitWhile(() => !isGameReset);
    }
    public IEnumerator CoFadeOut(UnityAction action = null)
    {
        fadeInOut.gameObject.SetActive(true);
        var black = Color.clear;

        var time = 0.5f;
        var timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;
            var value = Mathf.Lerp(0, 1, ratio);

            black.a = value;
            fadeInOut.color = black;
            yield return null;
        }

        action?.Invoke();
        yield return new WaitForSeconds(0.5f);

        timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;
            var value = Mathf.Lerp(1, 0, ratio);

            black.a = value;
            fadeInOut.color = black;
            yield return null;
        }
        fadeInOut.gameObject.SetActive(false);
    }
}
