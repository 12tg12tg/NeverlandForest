using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public enum StoryType
{
    Prologue,
    Chapter1,
    Chapter2,
    Chapter3,
    Chapter4,
    Chapter5,
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
    [Header("��ȭâ")]
    public GameObject messageBox;
    public TMP_Text talker;
    public TMP_Text talk;

    public GameObject MessageBox => messageBox;

    [Header("ȭ��ǥ UI")]
    [SerializeField] private RectTransform target;
    [SerializeField] private float distance;

    private Coroutine CoFade;
    private Vector2 startPos;

    public bool isNext;

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
        for (int i = 0; i < datas.Count; i++)
        {
            var data = (TutorialStoryDataTableElem)datas[i].Value;

            var description = data.description;
            var colorData = data.color;
            var typing = data.typing;
            var character = data.character;
            var fade = data.fade;

            talker.text = character == StoryChar.Hunter.ToString() ? "��ɲ�" :
                character == StoryChar.Herbalist.ToString() ? "��������" : "";

            if (colorData.Length > 0)
            {
                ColorUtility.TryParseHtmlString(colorData, out var color);
                talk.color = color;
            }
            if (typing)
            {
                for (int j = 0; j < description.Length; j++)
                {
                    talk.text += description[j];
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                talk.text = description;
            }
            yield return new WaitWhile(() => isNext);
            talk.text = "";
            talk.color = Color.white;
            isNext = false;
        }
        action?.Invoke();
    }
}
