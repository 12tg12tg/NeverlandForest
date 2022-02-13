using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 인벤토리 컨트롤러 제거, DataAllItem 속성들 다양해짐

public class RecipeIcon : MonoBehaviour
{
    [SerializeField] private List<RecipeObj> itemGoList = new List<RecipeObj>();
    private RecipeDataTable table;
    private AllItemTableElem fireobj;
    private AllItemTableElem condimentobj;
    private AllItemTableElem materialobj;
    private string Time = null;
    private string result = string.Empty;
    private bool isfireok;
    private bool iscondimentok;
    private bool ismaterialok;
    private int fireNum;
    private int condimentNum;
    private int materialNum;
    private DataAllItem fireitem;
    private DataAllItem condimentitem;
    private DataAllItem materialitem;
    private DataAllItem resultitem;
    private AllItemDataTable allitemTable;

    private int page = 1;
    [HideInInspector] public RecipeObj currentRecipe;
    private int maxPage;

    public Image fire;
    public Image condiment;
    public Image material;
    public TextMeshProUGUI makingTime;

    public Image resultItemIcon;
    public TextMeshProUGUI resultItemName;
    public TextMeshProUGUI resultItemDesc;

    public Image rewardresultItemIcon;
    public TextMeshProUGUI rewardresultItemName;
    public TextMeshProUGUI rewardresultItemDesc;


    [SerializeField] private Button previewButton;
    [SerializeField] private Button nextButton;

    public bool Isfireok => isfireok;
    public bool Iscondimentok => iscondimentok;
    public bool Ismaterialok => ismaterialok;
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        table = DataTableManager.GetTable<RecipeDataTable>();
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();

        var itemList = Vars.UserData.HaveRecipeIDList;

        for (int i = 0; i < 5; i++)
        {
            var index = i + 5 * (page - 1);
            if (index < itemList.Count)
            {
                itemGoList[i].Init(table, itemList[index], this);
            }
            else
            {
                itemGoList[i].Clear();
            }
        }
        SetPageButton();
    }
    public void SetPageButton()
    {
        if (page == 1)
        {
            previewButton.interactable = false;
        }
        else if (page == maxPage)
        {
            nextButton.interactable = false;
        }
        else
        {
            previewButton.interactable = true;
            nextButton.interactable = true;
        }
    }
    public void OnChangedSelection()
    {
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var fireid = $"ITEM_{(currentRecipe.Recipes[0])}";
        var condimentid = $"ITEM_{(currentRecipe.Recipes[1])}";
        var materialid = $"ITEM_{(currentRecipe.Recipes[2])}";

        fire.sprite = allitemTable.GetData<AllItemTableElem>(fireid).IconSprite;
        condiment.sprite = allitemTable.GetData<AllItemTableElem>(condimentid).IconSprite;
        if (condiment.sprite == null)
        {
            condiment.color = Color.clear;
        }
        else
        {
            condiment.color = Color.white;
        }
        material.sprite = allitemTable.GetData<AllItemTableElem>(materialid).IconSprite;
        if (material.sprite == null)
        {
            material.color = Color.clear;
        }
        else
        {
            condiment.color = Color.white;
        }
        result = currentRecipe.Result;

        fireobj = allitemTable.GetData<AllItemTableElem>(fireid);
        condimentobj = allitemTable.GetData<AllItemTableElem>(condimentid);
        materialobj = allitemTable.GetData<AllItemTableElem>(materialid);

        Time = currentRecipe.Time;
        makingTime.text = $"제작 시간은 {Time}분 입니다. ";
        var resultid = $"ITEM_{result}";
        resultItemIcon.sprite = allitemTable.GetData<AllItemTableElem>(resultid).IconSprite;
        resultItemIcon.color = Color.white;
        resultItemName.text = allitemTable.GetData<AllItemTableElem>(resultid).name;
        resultItemDesc.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;

        rewardresultItemIcon.sprite = allitemTable.GetData<AllItemTableElem>(resultid).IconSprite;
        rewardresultItemIcon.color = Color.white;
        rewardresultItemName.text = allitemTable.GetData<AllItemTableElem>(resultid).name;
        rewardresultItemDesc.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;


        CampManager.Instance.cookingText.text = "요리 하기";
    }
    public void MakeCooking()
    {
        var makeTime = int.Parse(Time);
        var list = Vars.UserData.HaveAllItemList;
        if (result != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ItemTableElem.id == fireobj.id)
                {
                    isfireok = true;
                    fireNum = i;
                }
                if (list[i].ItemTableElem.id == condimentobj.id)
                {
                    iscondimentok = true;
                    condimentNum = i;

                }
                if (list[i].ItemTableElem.id == materialobj.id)
                {
                    ismaterialok = true;
                    materialNum = i;
                }
            }
            if (!isfireok)
            {
                CampManager.Instance.cookingText.text = "재료가 부족합니다";
            }
            var zeroId = "ITEM_0";

            if (condimentobj.id == zeroId)
            {
                condimentitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(zeroId));
                iscondimentok = true;
            }

            if (materialobj.id == zeroId)
            {
                ismaterialok = true;
                materialitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(zeroId));
            }

            if (isfireok && iscondimentok && ismaterialok)
            {

                fireitem = new DataAllItem(list[fireNum]);
                fireitem.OwnCount = 1;

                if (condimentitem == null)
                {
                    condimentitem = new DataAllItem(list[condimentNum]);
                    condimentitem.OwnCount = 1;
                }
                if (materialitem == null)
                {
                    materialitem = new DataAllItem(list[materialNum]);
                    materialitem.OwnCount = 1;
                }

                var stringId = $"ITEM_{result}";
                resultitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringId));

                if (fireitem.itemId != zeroId)
                {
                    Vars.UserData.RemoveItemData(fireitem);
                }
                if (condimentitem.itemId != zeroId)
                {
                    Vars.UserData.RemoveItemData(condimentitem);
                }
                if (materialitem.itemId != zeroId)
                {
                    Vars.UserData.RemoveItemData(materialitem);
                }

                ConsumeManager.TimeUp(makeTime);
                ConsumeManager.RecoveryHunger(resultitem.ItemTableElem.stat_str);
                ConsumeManager.SaveConsumableData();
                isfireok = false;
                iscondimentok = false;
                ismaterialok = false;
                fire.sprite = null;
                condiment.sprite = null;
                material.sprite = null;
                result = string.Empty;
                makingTime.text = string.Empty;
                DiaryManager.Instacne.OpenCookingReward();
            }
        }
    }
    public void PreviewPageOpen()
    {
        if (page > 1)
        {
            page--;
            SetPageButton();
        }
    }
    public void NextPageOpen()
    {
        if (page < maxPage)
        {
            page++;
            SetPageButton();
        }
    }
}
