using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterUILinker : MonoBehaviour
{
    public GameObject hpBarPrefab;
    public MonsterUiInCanvas linkedUi;
    public Vector3 hpBarOffset = new Vector3(0f, 2.2f, 0f);
    private Canvas uiCanvas;

    private Image rangeImg;
    private Image hpBarImage;
    private Image sheildImg;
    private TextMeshProUGUI nextMoveDistance;

    public Color longRange;
    public Color shortRange;

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

    public void UpdateDistance(int speed)
    {
        nextMoveDistance.text = speed.ToString();
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
