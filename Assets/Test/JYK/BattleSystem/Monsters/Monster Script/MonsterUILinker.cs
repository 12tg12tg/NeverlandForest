using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterUILinker : MonoBehaviour
{
    public GameObject hpBarPrefab;

    // Component
    private Canvas uiCanvas;

    // Instance
    public MonsterUiInCanvas linkedUi;
    private Image rangeImg;
    private Image hpBarImage;
    private Image sheildImg;
    private Image iconImg;
    private TextMeshProUGUI nextMoveDistance;

    // Vars
    public Vector3 hpBarOffset = new Vector3(0f, 2.2f, 0f);
    public Color longRange;
    public Color shortRange;
    public Sprite attackIcon;
    public Sprite cantMoveIcon;

    public void Init(MonsterType type)
    {
        SetUI(type);
    }

    public void SetUI(MonsterType type)
    {
        uiCanvas = BattleManager.Instance.uiCanvas;
        var hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);
        linkedUi = hpBar.GetComponent<MonsterUiInCanvas>();

        rangeImg = linkedUi.rangeColor;
        hpBarImage = linkedUi.hpBarImg;
        sheildImg = linkedUi.sheildImg;
        nextMoveDistance = linkedUi.nextMoveDistance;
        iconImg = linkedUi.iconImage;
        linkedUi.Init(transform);

        hpBarOffset.y = GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.y;

        linkedUi.offset = hpBarOffset;

        SetRangeColor(type);
    }

    public void UpdateHpBar(int curHp, int fullHp)
    {
        hpBarImage.fillAmount = (float)curHp / fullHp;
    }

    public void UpdateSheild(int curS, int fulSild)
    {
        sheildImg.fillAmount = (float)curS / fulSild;
    }

    public void UpdateCircleUI(MonsterCommand command)
    {
        switch (command.actionType)
        {
            case MonsterActionType.None:
                nextMoveDistance.enabled = false;
                iconImg.enabled = true;
                iconImg.sprite = cantMoveIcon;
                break;
            case MonsterActionType.Attack:
                nextMoveDistance.enabled = false;
                iconImg.enabled = true;
                iconImg.sprite = attackIcon;
                break;
            case MonsterActionType.Move:
                nextMoveDistance.enabled = true;
                iconImg.enabled = false;
                nextMoveDistance.text = command.nextMove.ToString();
                break;
        }
    }

    public void SetRangeColor(MonsterType type)
    {
        switch (type)
        {
            case MonsterType.Near:
                rangeImg.color = shortRange;
                break;
            case MonsterType.Far:
                rangeImg.color = longRange;
                break;
        }
    }
}
