using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    private MultiTouch mt;

    [Header("UI 연결")]
    public BattleMessage message;
    public BattleMessage cautionMessage;
    public Image dragSlot;

    [Header("랜턴 범위 표시 이미지 연결")]
    public Transform lanternUITarget;

    [Header("드래그 중 인지 확인")]
    public bool isDrag;

    private int progress;
    public int Progress { get => progress; }

    private void Start()
    {
        mt = MultiTouch.Instance;
    }

    private void Update()
    {
        if (isDrag)
        {
            dragSlot.transform.position = mt.TouchPos;
        }
    }

    public void PrintMessage(string message, float time, UnityAction action)
    {
        this.message.PrintMessage(message, time, action);
    }

    public void PrintCaution(string message, float time, float fadeDelay, UnityAction action)
    {
        cautionMessage.PrintMessageFadeOut(message, 0.7f, 0.5f, action);
    }

    public void CreateTempSkillUiForDrag(DataPlayerSkill skill)
    {
        dragSlot.gameObject.SetActive(true);
        dragSlot.sprite = skill.SkillTableElem.IconSprite;
        isDrag = true;
    }

    public void CreateTempItemUiForDrag(DataAllItem item)
    {
        dragSlot.gameObject.SetActive(true);
        dragSlot.sprite = item.ItemTableElem.IconSprite;
        isDrag = true;
    }

    public void EndTempUiForDrag()
    {
        dragSlot.gameObject.SetActive(false);
        isDrag = false;
    }

    //Progress
    public void UpdateProgress()
    {
        progress++;
        BottomUIManager.Instance.UpdateProgress();
    }

    public void ResetProgress()
    {
        progress = 0;
        BottomUIManager.Instance.UpdateProgress();
    }
}
