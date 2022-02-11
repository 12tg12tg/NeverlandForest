using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentsRewards
{
    public readonly string itemID_Iron = "ITEM_9"; // 배틀
    public readonly string itemID_Axe = "ITEM_12"; // 배틀
    public readonly string itemID_Shovel = "ITEM_13"; // 배틀
    public readonly string itemID_Meat = "ITEM_7"; // 사냥
    public readonly string itemID_Egg = "ITEM_8"; // 사냥

    private AllItemTableElem ironElem;
    private AllItemTableElem axeElem;
    private AllItemTableElem shovelElem;
    private AllItemTableElem meatElem;
    private AllItemTableElem eggElem;

    private List<DataAllItem> huntRewards = new List<DataAllItem>();
    private List<DataAllItem> battleRewards = new List<DataAllItem>();

    public List<DataAllItem> GetBattleRewards(int monsterCount)
    {
        battleRewards.Clear();
        var iron = new DataAllItem(IronElem);
        iron.OwnCount = Mathf.CeilToInt(monsterCount / 2.0f);
        battleRewards.Add(iron);

        if(Random.Range(0f, 1f) < 0.1f)
        {
            if (Random.Range(0, 2) == 0)
            {
                var axe = new DataAllItem(AxeElem);
                axe.OwnCount = 1;
                battleRewards.Add(axe);
            }
            else
            {
                var shovel = new DataAllItem(ShovelElem);
                shovel.OwnCount = 1;
                battleRewards.Add(shovel);
            }
        }
        return battleRewards;
    }
    public List<DataAllItem> GetHuntRewards()
    {
        huntRewards.Clear();
        var meat = new DataAllItem(MeatElem);
        meat.OwnCount = 1;
        huntRewards.Add(meat);

        if (Random.Range(0f, 1f) < 0.5f)
        {
            var egg = new DataAllItem(EggElem);
            egg.OwnCount = 1;
            huntRewards.Add(egg);
        }
        return huntRewards;
    }

    public AllItemTableElem IronElem
    {
        get
        {
            if (ironElem == null)
                ironElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_Iron);
            return ironElem;
        }
    }
    public AllItemTableElem AxeElem
    {
        get
        {
            if (axeElem == null)
                axeElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_Axe);
            return axeElem;
        }
    }
    public AllItemTableElem ShovelElem
    {
        get
        {
            if (shovelElem == null)
                shovelElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_Shovel);
            return shovelElem;
        }
    }
    public AllItemTableElem MeatElem
    {
        get
        {
            if (meatElem == null)
                meatElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_Meat);
            return meatElem;
        }
    }
    public AllItemTableElem EggElem
    {
        get
        {
            if (eggElem == null)
                eggElem = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(itemID_Egg);
            return eggElem;
        }
    }
}
