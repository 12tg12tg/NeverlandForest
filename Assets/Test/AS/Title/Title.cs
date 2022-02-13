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
    private bool isFinish = false;
    public static bool isClear = false; // 얘는 월드맵에서 변경 시키고 들어오면 됨

    public TMP_Text startText;
    public Image panel;
    public Image fadeOut;
    public TMP_Text narration;
    public GameObject prologueWindow;
    public GameObject resetButton;

    private MultiTouch multiTouch;
    private StoryManager storyManager;

    public void Awake()
    {
        multiTouch = GameManager.Manager.MultiTouch;
        storyManager = GameManager.Manager.StoryManager;
        isFinish = GameManager.Manager.isClear;
    }

    public void Start()
    {
        SoundManager.Instance.Play(SoundType.BG_Intro);
    }

    private void Update()
    {
        coTapToStart ??= StartCoroutine(CoStartTextFadeIn(() => {
            coTapToStart = null;
            isStart = true;
        }));
        if (isClear) // 게임 클리어 후 월드맵에서 타이틀 화면으로 왔을 때 엔딩 스토리 실행
        {
            GameManager.Manager.Production.black.SetActive(false);
            prologueWindow.SetActive(true);
            storyManager.EndingStory(narration, () => resetButton.SetActive(true));
        }
        else if (isStart && multiTouch.TouchCount > 0)
        {
            var manager = GameManager.Manager;
            StartCoroutine(CoFadeOut(() => {
                gameObject.SetActive(false);
                if (isFinish) // 게임 클리어 후 리셋 버튼을 누르지 않고 게임을 껏다가 켰다면 여기로
                {
                    prologueWindow.SetActive(true);
                    storyManager.EndingStory(narration, () => resetButton.SetActive(true));
                }
                else
                    manager.TutoManager.Init();
            }, () => {
                if (!isFinish)
                    manager.Production.black.SetActive(true);
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

    private IEnumerator CoFadeOut(UnityAction action, UnityAction action2)
    {
        var isBlackOut = false;
        var black = Color.clear;

        var time = 1f;
        var timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;
            var value = Mathf.Lerp(0, 1, ratio);
            if (value > 0.9 && !isBlackOut)
            {
                isBlackOut = true;
                action2?.Invoke();
            }
            black.a = value;
            fadeOut.color = black;
            yield return null;
        }
        action?.Invoke();
        yield return new WaitForSeconds(1f);
    }

    public void GameReset() => isClear = false;
}
