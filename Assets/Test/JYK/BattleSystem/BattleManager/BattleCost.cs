using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCost : MonoBehaviour
{
    [Header("�Ŵ��� ����")]
    [SerializeField] private BattleManager bm;
    [SerializeField] private TileMaker tm;

    [Header("Ư�� ��ų ID �Է�")]
    public readonly string[] skillIDs_NearAttack = new string[] { "0", "5" };
    public readonly string skillID_knockBackAttack = "2";
    public readonly string skillID_focusAttack = "4";
    public readonly string skillID_threatEmission = "8";
    public readonly string skillID_chargeOil = "9";

    [Header("Ư�� ������ ID �Է�")]
    public readonly string itemID_oil = "ITEM_19";
    public readonly string itemID_arrow = "ITEM_20";
    public readonly string itemID_ironArrow = "ITEM_21";
    public readonly string itemID_potion = "ITEM_22"; // Ʃ�丮��
    public readonly string itemID_woodenTrap = "ITEM_16"; // Ʃ�丮��

    [Header("Ʈ�� ������")]
    public TrapSelecter trapSelector;

    [HideInInspector] public AllItemTableElem oilElem;
    [HideInInspector] public AllItemTableElem arrowElem;
    [HideInInspector] public AllItemTableElem ironArrowElem;
    [HideInInspector] public AllItemTableElem potionElem;
    [HideInInspector] public AllItemTableElem woodenTrapElem;

    private void Start()
    {
        oilElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_oil);
        arrowElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_arrow);
        ironArrowElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_ironArrow);
        potionElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_potion);
        woodenTrapElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_woodenTrap);
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
            if(skill.SkillTableElem.id == skillID_chargeOil) // ���� ����
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
        if(skill.SkillTableElem.id == skillID_chargeOil) // ���� ����
        {
            var dataItem = new DataAllItem(oilElem);
            dataItem.OwnCount = skill.SkillTableElem.cost;
            Vars.UserData.RemoveItemData(dataItem);
            BottomUIManager.Instance.ItemListInit();
        }
        else
        {
            var curLanternCount = Vars.UserData.uData.LanternCount;
            ConsumeManager.ConsumeLantern(skill.SkillTableElem.cost);
            bm.uiLink.UpdateLanternRange();
            Debug.Log($"���� ���Ͼ� {curLanternCount}���� {Vars.UserData.uData.LanternCount}�� �پ��.");
        }
    }

    public void CostForRecovery(DataAllItem item)
    {
        StartCoroutine(CoHeal(item));
    }

    private IEnumerator CoHeal(DataAllItem item)
    {
        // �ٸ� ������ Ŭ�� ���ϵ���
        tm.IsWaitingToHeal = true;
        // ��ŵ ���ϵ���
        bm.uiLink.turnSkipButton.interactable = false;

        // ��ƼŬ ��� - 2��
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

        yield return new WaitUntil(() => particleEnd); // ���

        // ü�� ȸ��
        ConsumeManager.RecoverHp(item.ItemTableElem.stat_Hp);

        // ������ �Ҹ�
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
