using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

[System.Obsolete("UI개편 이후 사용되지 않음.")]
public class BattleSkillInfo : MonoBehaviour
{
    //Component
    private RectTransform rt;

    //Instance
    private CanvasScaler cs;
    private DataPlayerSkill curSkill;
    private DataConsumable curItem;
    private BattleManager manager;

    //GmaeObject
    public ActionType curInfoType;
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
        //cs ??= manager.cs;

        curSkill = skill;
        var elem = curSkill.SkillTableElem;
        iconImg.sprite = elem.IconSprite;
        iconName.text = elem.name;

        sb.Clear();
        var count = elem.cost;
        var costItem = elem.player == PlayerType.Boy ? "화살" : "랜턴";
        if (count == -1)
            sb.Append($"소모 : 제한없음.\n");
        else
            sb.Append($"소모 : {costItem} {elem.cost}개\n");
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

    public void Init(DataConsumable item, Vector2 pos)
    {
        manager ??= BattleManager.Instance;
        //cs ??= manager.cs;

        curItem = item;
        var elem = curItem.ItemTableElem;
        iconImg.sprite = elem.IconSprite;
        iconName.text = elem.name;

        sb.Clear();
        var count = item.OwnCount;
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
        //var button = manager.CurClickedButton;
        //button.groupUI.DisableGroupDuringSelect(button);

        //if (button.CurState == ActionType.Skill)
        //{
        //    manager.DisplayMonsterTile(curSkill.SkillTableElem.range);
        //}
        //else
        //{
        //    manager.DisplayPlayerTile();
        //}
        //manager.ReadyTileClick();
        //manager.PrintCaution("사용할 지점을 터치하세요.", 1f, 0.7f, null);
        //gameObject.SetActive(false);
    }

    private bool IsContainPos(Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, pos);
    }
}
