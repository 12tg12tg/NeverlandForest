using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 인벤토리 컨트롤러 제거, DataAllItem 속성들 다양해짐

public class RecipeIcon : MonoBehaviour
{
    [SerializeField] private List<RecipeObj> itemGoList = new List<RecipeObj>();
    private string Time = null;
    private string result = string.Empty;

    private int fireNum;
    private int condimentNum;
    private int materialNum;

    private RecipeDataTable recipetable;
    private AllItemTableElem fireobj;
    private AllItemTableElem condimentobj;
    private AllItemTableElem materialobj;

    private DataAllItem fireitem;
    private DataAllItem condimentitem;
    private DataAllItem materialitem;
    private DataAllItem resultitem;
    private AllItemDataTable allitemTable;
    private int page = 1;

    [HideInInspector] public RecipeObj currentRecipe;
    private readonly int maxPage;

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

    private bool isfireok;
    private bool iscondimentok;
    private bool ismaterialok;
    public bool Isfireok => isfireok;
    public bool Iscondimentok => iscondimentok;
    public bool Ismaterialok => ismaterialok;
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        recipetable = DataTableManager.GetTable<RecipeDataTable>();
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var haverecipeList = Vars.UserData.HaveRecipeIDList;
        for (int i = 0; i < 5; i++)
        {
            var index = i + 5 * (page - 1);
            if (index < haverecipeList.Count)
                itemGoList[i].Init(recipetable, haverecipeList[index], this);
            else
                itemGoList[i].Clear();
        }
        SetPageButton();
    }
    public void SetPageButton()
    {
        if (page == 1)
            previewButton.interactable = false;
        else if (page == maxPage)
            nextButton.interactable = false;
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
        result = currentRecipe.Result;

        var resultid = $"ITEM_{result}";
        var userItemList = Vars.UserData.HaveAllItemList;
        var zeroId = "ITEM_0";

        fire.sprite = allitemTable.GetData<AllItemTableElem>(fireid).IconSprite;
        if (fire.sprite !=null)
            fire.color = Color.white;
        condiment.sprite = allitemTable.GetData<AllItemTableElem>(condimentid).IconSprite;
        if (condiment.sprite != null)
            condiment.color = Color.white;
        else
            condiment.color = Color.clear;
        material.sprite = allitemTable.GetData<AllItemTableElem>(materialid).IconSprite;
        if (material.sprite != null)
            material.color = Color.white;
        else
            material.color = Color.clear;
        fireobj = allitemTable.GetData<AllItemTableElem>(fireid);
        condimentobj = allitemTable.GetData<AllItemTableElem>(condimentid);
        materialobj = allitemTable.GetData<AllItemTableElem>(materialid);

        fireitem = new DataAllItem(fireobj);
        fireitem.OwnCount = 1;

        condimentitem = new DataAllItem(condimentobj);
        condimentitem.OwnCount = 1;

        materialitem = new DataAllItem(materialobj);
        materialitem.OwnCount = 1;

        if (condimentobj.id == zeroId)
        {
            iscondimentok = true;
        }
        else
        {
            iscondimentok = false;
        }
        if (materialobj.id == zeroId)
        {
            ismaterialok = true;
        }
        else
        {
            ismaterialok = false;
        }

        resultitem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(resultid));
        resultItemIcon.sprite = allitemTable.GetData<AllItemTableElem>(resultid).IconSprite;
        resultItemIcon.color = Color.white;
        resultItemName.text = allitemTable.GetData<AllItemTableElem>(resultid).name;
        resultItemDesc.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;

        rewardresultItemIcon.sprite = allitemTable.GetData<AllItemTableElem>(resultid).IconSprite;
        rewardresultItemIcon.color = Color.white;
        rewardresultItemName.text = allitemTable.GetData<AllItemTableElem>(resultid).name;
        rewardresultItemDesc.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;

        Time = currentRecipe.Time;
        makingTime.text = $"제작 시간은 {Time}분 입니다. ";
      
        for (int i = 0; i < userItemList.Count; i++)
        {
            if (userItemList[i].ItemTableElem.id == fireobj.id)
            {
                isfireok = true;
                fireNum = i;
            }
        }

       CampManager.Instance.cookingText.text = "요리 하기";
    }
    public void MakeCooking()
    {
        var userItemList = Vars.UserData.HaveAllItemList;
        var zeroId = "ITEM_0";
        if (result != null)
        {
            for (int i = 0; i < userItemList.Count; i++)
            {
                if (userItemList[i].ItemTableElem.id == condimentobj.id)
                {
                    condimentNum = i;
                    iscondimentok = true;
                }
                if (userItemList[i].ItemTableElem.id == materialobj.id)
                {
                    materialNum = i;
                    ismaterialok = true;
                }
            }
            if (!isfireok || !iscondimentok || ismaterialok)
            {
                CampManager.Instance.cookingText.text = "재료가 부족합니다";
            }
            var resultid = $"ITEM_{result}";
            var consumeTime = allitemTable.GetData<AllItemTableElem>(resultid).duration;
            var haveBonfireTime = Vars.UserData.uData.BonfireHour * 60;

            if (isfireok && iscondimentok && ismaterialok && haveBonfireTime>=consumeTime)
            {

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
                if (DiaryManager.Instacne.IsRotation && ismaterialok && iscondimentok)
                {
                    DiaryManager.Instacne.CookingRotationPanel.SetActive(true);
                }
                ConsumeManager.TimeUp(consumeTime);
                Vars.UserData.uData.BonfireHour = (haveBonfireTime - consumeTime) / 60;
                if (Vars.UserData.uData.BonfireHour<=0)
                {
                    Vars.UserData.uData.BonfireHour = 0;
                }
                ConsumeManager.RecoveryHunger(resultitem.ItemTableElem.stat_str);
                ConsumeManager.SaveConsumableData();
                CampManager.Instance.SetBonTime();
                CookReset();
                DiaryManager.Instacne.OpenCookingReward();
            }
        }
        CampManager.Instance.camptutorial.TutorialCook_startButton = true;
    }
    public void PreviewPageOpen() //버튼함수
    {
        if (page > 1)
        {
            page--;
            SetPageButton();
        }
    }
    public void NextPageOpen() //버튼함수
    {
        if (page < maxPage)
        {
            page++;
            SetPageButton();
        }
    }

    public void CookReset()
    {
        isfireok = false;
        iscondimentok = false;
        ismaterialok = false;

        fire.sprite = null;
        fire.color = Color.clear;
        condiment.sprite = null;
        condiment.color = Color.clear;

        material.sprite = null;
        material.color = Color.clear;

        result = string.Empty;
        makingTime.text = string.Empty;
    }
}
