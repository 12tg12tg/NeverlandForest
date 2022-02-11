using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleCost : MonoBehaviour
{
    [Header("매니저 연결")]
    [SerializeField] private BattleManager bm;
    [SerializeField] private TileMaker tm;

    [Header("특수 스킬 ID 입력")]
    public readonly string[] skillIDs_NearAttack = new string[] { "0", "5" };
    public readonly string skillID_knockBackAttack = "2";
    public readonly string skillID_focusAttack = "4";
    public readonly string skillID_threatEmission = "8";
    public readonly string skillID_chargeOil = "9";

    [Header("특수 아이템 ID 입력")]
    public readonly string itemID_oil = "ITEM_19";
    public readonly string itemID_arrow = "ITEM_20";
    public readonly string itemID_ironArrow = "ITEM_21";
    public readonly string itemID_potion = "ITEM_22"; // 튜토리얼
    public readonly string itemID_woodenTrap = "ITEM_16"; // 튜토리얼

    [Header("트랩 생성기")]
    public TrapSelecter trapSelector;

    private AllItemTableElem oilElem;
    private AllItemTableElem arrowElem;
    private AllItemTableElem ironArrowElem;
    private AllItemTableElem potionElem;
    private AllItemTableElem woodenTrapElem;

    public AllItemTableElem OilElem
    {
        get
        {
            if (oilElem == null)
                oilElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_oil);
            return oilElem;
        }
    }
    public AllItemTableElem ArrowElem
    {
        get
        {
            if (arrowElem == null)
                arrowElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_arrow);
            return arrowElem;
        }
    }
    public AllItemTableElem IronArrowElem
    {
        get
        {
            if (ironArrowElem == null)
                ironArrowElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_ironArrow);
            return ironArrowElem;
        }
    }
    public AllItemTableElem PotionElem
    {
        get
        {
            if (potionElem == null)
                potionElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_potion);
            return potionElem;
        }
    }
    public AllItemTableElem WoodenTrapElem
    {
        get
        {
            if (woodenTrapElem == null)
                woodenTrapElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_woodenTrap);
            return woodenTrapElem;
        }
    }

    // 화살 elme 얻기
    public AllItemTableElem GetCurrentArrowElem()
    {
        if (Vars.UserData.arrowType == ArrowType.Normal)
            return ArrowElem;
        else if (Vars.UserData.arrowType == ArrowType.Iron)
            return IronArrowElem;
        else
            return null;
    }

    public AllItemTableElem GetOtherArrowElem()
    {
        if (Vars.UserData.arrowType == ArrowType.Normal)
            return IronArrowElem;
        else if (Vars.UserData.arrowType == ArrowType.Iron)
            return ArrowElem;
        else
            return null;
    }


    // 인벤토리 보유 상황 확인
    public bool HaveArrowNow()
    {
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_arrow)
            {
                return true;
            }
        }
        return false;
    }

    public bool HaveIronArrowNow()
    {
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_ironArrow)
            {
                return true;
            }
        }
        return false;
    }

    public bool HaveOilNow()
    {
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_oil)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckCanUseSkill(DataPlayerSkill skill, out string cautionMessage)
    {
        if(skill.SkillTableElem.player == PlayerType.Boy)
        {
            if (Vars.UserData.arrowType == ArrowType.Normal)
            {
                cautionMessage = "스킬에 필요한 화살이 부족합니다.";
                return CheckNormalArrow(skill);
            }
            else
            {
                cautionMessage = "스킬에 필요한 쇠화살이 부족합니다.";
                return CheckIronlArrow(skill);
            }
        }
        else
        {
            if(skill.SkillTableElem.id == skillID_chargeOil) // 오일 충전
            {
                cautionMessage = "스킬에 필요한 오일이 부족합니다.";
                return CheckOil(skill);
            }
            else
            {
                cautionMessage = "스킬에 필요한 랜턴밝기가 부족합니다.";
                return CheckLantern(skill);
            }
        }
    }

    public int NumberOfArrows()
    {
        if (Vars.UserData.arrowType == ArrowType.Normal)
            return NumberOfArrow();
        else
            return NumberOfIronArrow();
    }
    private int NumberOfArrow()
    {
        int total = 0;
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_arrow)
            {
                total += item.OwnCount;
            }
        }
        return total;
    }

    private int NumberOfIronArrow()
    {
        int total = 0;
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_ironArrow)
            {
                total += item.OwnCount;
            }
        }
        return total;
    }

    public int NumberOfOil()
    {
        int total = 0;
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_oil)
            {
                total += item.OwnCount;
            }
        }
        return total;
    }

    private bool CheckNormalArrow(DataPlayerSkill skill)
    {
        int total = 0;
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_arrow)
            {
                total += item.OwnCount;
            }
        }
        return total >= skill.SkillTableElem.cost;
    }

    private bool CheckIronlArrow(DataPlayerSkill skill)
    {
        int total = 0;
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_ironArrow)
            {
                total += item.OwnCount;
            }
        }
        return total >= skill.SkillTableElem.cost;
    }

    private bool CheckOil(DataPlayerSkill skill)
    {
        int total = 0;
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == itemID_oil)
            {
                total += item.OwnCount;
            }
        }
        return total >= skill.SkillTableElem.cost;
    }

    private bool CheckLantern(DataPlayerSkill skill)
    {
        return Vars.UserData.uData.LanternCount >= skill.SkillTableElem.cost;
    }

    // 소모 함수
    public void CostArrow(DataPlayerSkill skill)
    {
        AllItemTableElem costArrow;
        if (Vars.UserData.arrowType == ArrowType.Normal)
            costArrow = ArrowElem;
        else
            costArrow = IronArrowElem;

        var dataItem = new DataAllItem(costArrow);
        dataItem.OwnCount = skill.SkillTableElem.cost;
        Vars.UserData.RemoveItemData(dataItem);
        BottomUIManager.Instance.ItemListInit();
    }

    public void CostLanternOrOil(DataPlayerSkill skill)
    {
        if(skill.SkillTableElem.id == skillID_chargeOil) // 오일 충전
        {
            var dataItem = new DataAllItem(OilElem);
            dataItem.OwnCount = skill.SkillTableElem.cost;
            Vars.UserData.RemoveItemData(dataItem);
            BottomUIManager.Instance.ItemListInit();
        }
        else
        {
            var curLanternCount = Vars.UserData.uData.LanternCount;
            ConsumeManager.ConsumeLantern(skill.SkillTableElem.cost);
            bm.uiLink.UpdateLanternRange();
            Debug.Log($"원래 랜턴양 {curLanternCount}에서 {Vars.UserData.uData.LanternCount}로 줄어듦.");
        }
    }

    public void CostForRecovery(DataAllItem item)
    {
        StartCoroutine(CoHeal(item));
    }

    private IEnumerator CoHeal(DataAllItem item)
    {
        // 다른 아이템 클릭 못하도록
        tm.IsWaitingToHeal = true;
        // 스킵 못하도록
        bm.uiLink.turnSkipButton.interactable = false;

        // 파티클 재생 - 2개
        Particle script;
        bool isAction = false;
        bool particleEnd = false;
        if (bm.boy.FSM.curState != CharacterBattleState.Death)
        {
            var go = ProjectilePool.Instance.GetObject(ProjectileTag.Heal);
            go.transform.SetParent(bm.projectileParent);
            script = go.GetComponent<Particle>();
            var pos = bm.boy.transform.position;
            pos.y = 0.02f;
            go.transform.position = pos;

            isAction = true;
            script.Init(() => particleEnd = true);
        }
        if(bm.girl.FSM.curState != CharacterBattleState.Death)
        {
            var go = ProjectilePool.Instance.GetObject(ProjectileTag.Heal);
            go.transform.SetParent(bm.projectileParent);
            script = go.GetComponent<Particle>();
            var pos = bm.girl.transform.position;
            pos.y = 0.02f;
            go.transform.position = pos;

            if (!isAction)
            {
                isAction = true;
                script.Init(() => particleEnd = true);
            }
            else
            {
                script.Init();
            }
        }

        yield return new WaitUntil(() => particleEnd); // 대기

        // 체력 회복
        ConsumeManager.RecoverHp(item.ItemTableElem.stat_Hp);

        // 아이템 소모
        var allItem = new DataAllItem(item);
        allItem.OwnCount = 1;
        Vars.UserData.RemoveItemData(allItem);
        BottomUIManager.Instance.ItemListInit();

        bm.uiLink.UpdateProgress(BattleUI.ProgressIcon.Potion);
        bm.EndOfPlayerAction();

        tm.IsWaitingToHeal = false;
        bm.uiLink.turnSkipButton.interactable = true;
    }
}
