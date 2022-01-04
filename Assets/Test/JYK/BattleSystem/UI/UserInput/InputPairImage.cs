using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputPairImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CanvasGroup cg;
    public UserInputPanel panel;
    public Image playerImg;
    public Image choiceImg;

    private int sibilingIndex;

    public void Init(PlayerType type, DataPlayerSkill skill)
    {
        if (type == PlayerType.Boy)
        {
            ColorUtility.TryParseHtmlString("#42C0FF", out Color color);
            playerImg.color = color;
        }
        else
        {
            ColorUtility.TryParseHtmlString("#FFCC42", out Color color);
            playerImg.color = color;
        }

        choiceImg.sprite = skill.SkillTableElem.IconSprite;
    }
    public void Init(PlayerType type, DataConsumable item)
    {
        if (type == PlayerType.Boy)
        {
            ColorUtility.TryParseHtmlString("#42C0FF", out Color color);
            playerImg.color = color;
        }
        else
        {
            ColorUtility.TryParseHtmlString("#FFCC42", out Color color);
            playerImg.color = color;
        }

        choiceImg.sprite = item.ItemTableElem.IconSprite;
    }
    public void Init(InputPairImage another)
    {
        playerImg.color = another.playerImg.color;
        choiceImg.sprite = another.choiceImg.sprite;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!panel.IsBothInputPresent)
            return;

        cg.alpha = 0.5f;
        var temp = panel.dragSlot;
        temp.Init(this);
        temp.gameObject.SetActive(true);
        temp.transform.position = eventData.position;

        sibilingIndex = transform.parent.GetSiblingIndex();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!panel.IsBothInputPresent)
            return;

        panel.dragSlot.transform.position = eventData.position;
        var sibilingIndex = transform.parent.GetSiblingIndex();
        if(sibilingIndex == 0)
        {
            if (eventData.position.x > panel.secondInput.transform.position.x)
            {
                transform.parent.SetSiblingIndex(1);
            }
        }
        else
        {
            if (eventData.position.x < panel.firstInput.transform.position.x)
            {
                transform.parent.SetSiblingIndex(0);
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!panel.IsBothInputPresent)
            return;

        cg.alpha = 1f;
        panel.dragSlot.gameObject.SetActive(false);

        var sibilingIndexAfterDrag = transform.parent.GetSiblingIndex();

        if(sibilingIndex != sibilingIndexAfterDrag)
        {
            panel.Swap();
            BattleManager.Instance.CommandArrangeSwap();
        }
    }
}
