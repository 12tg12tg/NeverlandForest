using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillSelectUI : MonoBehaviour
{
    //prefab
    public GameObject skillPrefab;

    //Component
    private CanvasGroup group;
    public RectTransform rt;
    public HorizontalLayoutGroup categoryHLG;

    //Instance
    private CanvasScaler cs;
    private SkillSelectUI another;
    private BattleManager manager;
    public Transform skillContent;
    private CanvasGroup exceptGoDuringDrag;
    public Image skillTap;
    public Image itemTap;
    public Image panel;

    //Own List
    private List<DataPlayerSkill> skillList;
    private List<SkillButton> skillGoList = new List<SkillButton>();

    //Vars
    private int defaultCount = 20;
    private float popupTime = 0.4f;
    private float popdownTime = 0.2f;
    private ActionType curSlot;
    public PlayerType type;

    //Property
    private Vector3 BelowScreen 
    { 
        get
        {
            var v3 = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0f, 0f));
            v3.y -= Height;
            return v3;
        }
    }
    private float Height
    {
        get
        {
            float wRatio = Screen.width / cs.referenceResolution.x;
            float hRatio = Screen.height / cs.referenceResolution.y;

            float ratio =
                wRatio * (1f - cs.matchWidthOrHeight) +
                hRatio * (cs.matchWidthOrHeight);
            return rt.rect.height * ratio;
        }
    }

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        categoryHLG.enabled = false;
        skillTap.transform.parent.parent.SetSiblingIndex(1);
    }

    //초기화
    public void Init(PlayerType type, BattleManager manager, List<DataPlayerSkill> skills, SkillSelectUI anotherUi)
    {
        this.type = type;
        skillList = skills;
        for (int i = 0; i < defaultCount; i++)
        {
            var go = Instantiate(skillPrefab, skillContent);
            go.name = $"Skill {i}";
            go.SetActive(false);
            skillGoList.Add(go.GetComponent<SkillButton>());
        }


        this.manager = manager;
        cs = manager.cs;
        another = anotherUi;

        ChangeSlot(ActionType.Skill);
    }
    private void SetAllGoUnactivate()
    {
        for (int i = 0; i < defaultCount; i++)
        {
            skillGoList[i].gameObject.SetActive(false);
        }
    }
    public void SetGoToItem()
    {
        SetAllGoUnactivate();
        var itemList = Vars.ConsumableItemList;
        for (int i = 0; i < itemList.Count; i++)
        {
            skillGoList[i].gameObject.SetActive(true);
            skillGoList[i].Init(this, manager, itemList[i]);
        }
    }
    public void SetGoToSkill()
    {
        SetAllGoUnactivate();
        for (int i = 0; i < skillList.Count; i++)
        {
            skillGoList[i].gameObject.SetActive(true);
            skillGoList[i].Init(this, manager, skillList[i]);
        }
    }

    //Open Close
    public void Open()
    {
        if (gameObject.activeInHierarchy)
            return;

        gameObject.SetActive(true);
        transform.position = BelowScreen;
        StartCoroutine(CoOpenThisUp());
    }
    private IEnumerator CoOpenThisUp()
    {
        if (another.gameObject.activeInHierarchy)
        {
            yield return StartCoroutine(CoCloseDown(another.rt));
        }

        float timer = 0f;
        Vector3 startPos = rt.position;
        float startY = startPos.y;
        float endY = startY + Height;
        while (timer <= popupTime)
        {
            timer += Time.deltaTime;
            var ratio = timer / popupTime;
            var lerp = Mathf.Lerp(startY, endY, ratio);
            rt.position = new Vector3(startPos.x, lerp, startPos.z);
            yield return null;
        }
        rt.position = new Vector3(startPos.x, endY, startPos.z);
    }
    private IEnumerator CoCloseDown(RectTransform target)
    {
        float timer = 0f;
        Vector3 startPos = target.position;
        float startY = startPos.y;
        float endY = BelowScreen.y;
        while (timer <= popdownTime)
        {
            timer += Time.deltaTime;
            var ratio = timer / popdownTime;
            var lerp = Mathf.Lerp(startY, endY, ratio);
            target.position = new Vector3(startPos.x, lerp, startPos.z);
            yield return null;
        }
        target.position = new Vector3(startPos.x, endY, startPos.z);
        target.gameObject.SetActive(false);
    }


    //드래그 드랍 관련
    public void DisableGroupDuringDrag(SkillButton except)
    {
        exceptGoDuringDrag = except.GetComponent<CanvasGroup>();
        exceptGoDuringDrag.ignoreParentGroups = true;

        group.interactable = false;
        group.alpha = 0.5f;
    }
    public void EnableGroup()
    {
        if(exceptGoDuringDrag != null)
            exceptGoDuringDrag.ignoreParentGroups = false;
        group.interactable = true;
        group.alpha = 1f;
    }

    //탭 전환
    public void ChangeSlot(ActionType slot)
    {
        if (slot == ActionType.Skill)
        {
            SetGoToSkill();
            panel.color = skillTap.color;
        }
        else
        {
            SetGoToItem();
            panel.color = itemTap.color;
        }

        curSlot = slot;
    }
    public void OnClickSkillTap()
    {
        if (curSlot != ActionType.Skill)
        {
            ChangeSlot(ActionType.Skill);
            skillTap.transform.parent.parent.SetSiblingIndex(1);
        }
    }
    public void OnClickItemTap()
    {
        if (curSlot != ActionType.Item)
        {
            ChangeSlot(ActionType.Item);
            itemTap.transform.parent.parent.SetSiblingIndex(1);
        }
    }
}
