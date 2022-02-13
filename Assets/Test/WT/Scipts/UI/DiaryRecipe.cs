using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiaryRecipe : MonoBehaviour
{
    [SerializeField] private List<RecipeObject> itemGoList = new List<RecipeObject>();
    private RecipeDataTable table;
    private AllItemDataTable allitemTable;
    private string Time = null;
    private string result = string.Empty;
    [Header("텍스트 셋팅")]
    public TextMeshProUGUI makingTime;
    public TextMeshProUGUI resultName;
    public TextMeshProUGUI cookeffect;
    public TextMeshProUGUI description;
    [Header("이미지 셋팅")]
    public Image recipecookImage;
    public Image material0;
    public Image material1;
    public Image material2;
    private int page = 1;
    private int maxPage;
    [HideInInspector] public RecipeObject currentRecipe;
    [SerializeField] private Button previewButton;
    [SerializeField] private Button nextButton;
    public void Start()
    {
        Init();
    }
    public void Init()
    {
        table = DataTableManager.GetTable<RecipeDataTable>();
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var itemList = Vars.UserData.HaveRecipeIDList;

        for (int i = 0; i < 16; i++)
        {
            var index = i + 16 * (page - 1);
            if (index < itemList.Count)
            {
                itemGoList[i].Init(table, itemList[index], this);
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
        var fireid = string.Empty;
        var condimentid = string.Empty;
        var materialid = string.Empty;
        if (currentRecipe.Recipes != null)
        {
            if (currentRecipe.Recipes[0] != null)
            {
                fireid = $"ITEM_{(currentRecipe.Recipes[0])}";
                material0.sprite = allitemTable.GetData<AllItemTableElem>(fireid).IconSprite;
            }
            if (currentRecipe.Recipes[1] != null)
            {
                condimentid = $"ITEM_{(currentRecipe.Recipes[1])}";
                var condimentSprite = allitemTable.GetData<AllItemTableElem>(condimentid).IconSprite;
                if (condimentSprite == null)
                {
                    material1.color = Color.clear;
                }
                else
                {
                    material1.sprite = condimentSprite;
                }
          
            }
            if (currentRecipe.Recipes[2] != null)
            {
                materialid = $"ITEM_{(currentRecipe.Recipes[2])}";
                var materialSprite = allitemTable.GetData<AllItemTableElem>(materialid).IconSprite;
                if (materialSprite == null)
                {
                    material2.color = Color.clear;
                }
                else
                {
                    material2.sprite = materialSprite;
                }
            }
            result = currentRecipe.Result;
            var resultid = $"ITEM_{result}";
            recipecookImage.sprite = allitemTable.GetData<AllItemTableElem>(resultid).IconSprite;
            Time = currentRecipe.Time;
            makingTime.text = $"제작 시간은 {Time}분 입니다. ";
            resultName.text = allitemTable.GetData<AllItemTableElem>(resultid).name;
            cookeffect.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;
            description.text = allitemTable.GetData<AllItemTableElem>(resultid).desc;
        }
    }
    public void PreviewPageOpen()
    {
        if (page > 1)
        {
            page--;
            SetPageButton();
        }
        SoundManager.Instance.Play(SoundType.Se_Diary);
    }
    public void NextPageOpen()
    {
        if (page < maxPage)
        {
            page++;
            SetPageButton();
        }
        SoundManager.Instance.Play(SoundType.Se_Diary);
    }
}
