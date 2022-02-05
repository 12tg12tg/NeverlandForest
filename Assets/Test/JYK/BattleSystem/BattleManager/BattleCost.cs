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

    // ȭ�� elme ���
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


    // �κ��丮 ���� ��Ȳ Ȯ��
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
                cautionMessage = "��ų�� �ʿ��� ȭ���� �����մϴ�.";
                return CheckNormalArrow(skill);
            }
            else
            {
                cautionMessage = "��ų�� �ʿ��� ��ȭ���� �����մϴ�.";
                return CheckIronlArrow(skill);
            }
        }
        else
        {
            if(skill.SkillTableElem.name == "���� ����")
            {
                cautionMessage = "��ų�� �ʿ��� ������ �����մϴ�.";
                return CheckOil(skill);
            }
            else
            {
                cautionMessage = "��ų�� �ʿ��� ���Ϲ�Ⱑ �����մϴ�.";
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

    // �Ҹ� �Լ�
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
        if(skill.SkillTableElem.name == "���� ����")
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
