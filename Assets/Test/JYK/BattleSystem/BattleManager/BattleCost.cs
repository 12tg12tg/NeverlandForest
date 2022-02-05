using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCost : MonoBehaviour
{
    [SerializeField] private BattleManager bm;
    public readonly string oilID = "ITEM_19";
    public readonly string arrowID = "ITEM_20";
    public readonly string ironArrowID = "ITEM_21";
    [HideInInspector] public AllItemTableElem oilElem;
    [HideInInspector] public AllItemTableElem arrowElem;
    [HideInInspector] public AllItemTableElem ironArrowElem;

    private void Start()
    {
        oilElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(oilID);
        arrowElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(arrowID);
        ironArrowElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(ironArrowID);
    }

    // 화살 elme 얻기
    public AllItemTableElem GetCurrentArrowElem()
    {
        if (Vars.UserData.arrowType == ArrowType.Normal)
            return arrowElem;
        else if (Vars.UserData.arrowType == ArrowType.Iron)
            return ironArrowElem;
        else
            return null;
    }

    public AllItemTableElem GetOtherArrowElem()
    {
        if (Vars.UserData.arrowType == ArrowType.Normal)
            return ironArrowElem;
        else if (Vars.UserData.arrowType == ArrowType.Iron)
            return arrowElem;
        else
            return null;
    }


    // 인벤토리 보유 상황 확인
    public bool HaveArrowNow()
    {
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == arrowID)
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
            if (item.itemId == ironArrowID)
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
            if (item.itemId == oilID)
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
            if(skill.SkillTableElem.name == "오일 충전")
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
            if (item.itemId == arrowID)
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
            if (item.itemId == ironArrowID)
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
            if (item.itemId == oilID)
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
            if (item.itemId == arrowID)
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
            if (item.itemId == ironArrowID)
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
            if (item.itemId == oilID)
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
            costArrow = arrowElem;
        else
            costArrow = ironArrowElem;

        var dataItem = new DataAllItem(costArrow);
        dataItem.OwnCount = skill.SkillTableElem.cost;
        Vars.UserData.RemoveItemData(dataItem);
        BottomUIManager.Instance.ItemListInit();
    }

    public void CostLanternOrOil(DataPlayerSkill skill)
    {
        if(skill.SkillTableElem.name == "오일 충전")
        {
            var dataItem = new DataAllItem(oilElem);
            dataItem.OwnCount = skill.SkillTableElem.cost;
            Vars.UserData.RemoveItemData(dataItem);
            BottomUIManager.Instance.ItemListInit();
        }
        else
        {
            Vars.UserData.uData.LanternCount -= skill.SkillTableElem.cost;
        }
    }
}
