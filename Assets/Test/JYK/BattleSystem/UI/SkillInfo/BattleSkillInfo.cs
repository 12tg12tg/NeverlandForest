using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class BattleSkillInfo : MonoBehaviour
{
    //Component
    private RectTransform rt;

    //Instance
    private CanvasScaler cs;
    private DataPlayerSkill curSkill;
    private DataCunsumable curItem;
    private BattleManager manager;

    //GmaeObject
    private ActionType curInfoType;
    public Image iconImg;
    public TextMeshProUGUI iconName;
    public TextMeshProUGUI description;
    private StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        var touchPos = MultiTouch.Instance.TouchPos;
        if (touchPos != Vector2.zero && !IsContainPos(touchPos))
            gameObject.SetActive(false);
    }

    public void Init(DataPlayerSkill skill, Vector2 pos)
    {
        manager ??= BattleManager.Instance;
        cs ??= manager.cs;

        curSkill = skill;
        var elem = curSkill.SkillTableElem;
        iconImg.sprite = elem.IconSprite;
        iconName.text = elem.name;

        sb.Clear();
        var count = elem.count;
        if (count == -1)
            sb.Append($"횟수 : 제한없음.\n");
        else
            sb.Append($"횟수 : {elem.count}\n");
        sb.Append($"{elem.description}");
        description.text = sb.ToString();
        curInfoType = ActionType.Skill;

        transform.position = pos;

        var width = Utility.RelativeRectSize(cs, rt).x;
        if (Screen.width < pos.x + width)
        {
            pos.x = Screen.width - width;
        }
        transform.position = pos;
    }

    public void Init(DataCunsumable item, Vector2 pos)
    {
        manager ??= BattleManager.Instance;
        cs ??= manager.cs;

        curItem = item;
        var elem = curItem.ItemTableElem;
        iconImg.sprite = elem.IconSprite;
        iconName.text = elem.name;

        sb.Clear();
        var count = item.count;
        sb.Append($"보유 수량 : {count}\n");
        sb.Append($"{elem.description}");
        description.text = sb.ToString();
        curInfoType = ActionType.Item;

        var width = Utility.RelativeRectSize(cs, rt).x;
        if (Screen.width < pos.x + width)
        {
            pos.x = Screen.width - width;
        }
        transform.position = pos;
    }

    public void CloseInfo()
    {
        gameObject.SetActive(false);
    }

    public void SelectSkill()
    {
        Debug.Log($"Select {curSkill.SkillTableElem.name}");
        //DataPlayerSkill 전달
    }

    private bool IsContainPos(Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, pos);
    }
}
