using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiaryManager : MonoBehaviour
{
    [Header("태그관련")]
    public Image produceTagImage;
    public Image cookingTagImage;
    public Image sleepingTagImage;
    public Image gatheringInCampTagImage;
    public Image battleTagImage;
    public Image huntTagImage;
    public Image gatheringInDungeonTagImage;

    [Header("판넬관련")]
    public GameObject producePanel;
    public GameObject cookingPanel;
    public GameObject sleepingPanel;
    public GameObject gatheringIncCampPanel;
    public GameObject gatheringInCampRewardPanel;
    public GameObject battleRewardPanel;
    public GameObject huntRewardPanel;
    public GameObject gatheringInDungeonPanel;
    public GameObject gatheringInDungeonRewardPanel;
    public GameObject CookingRotationPanel;
    public GameObject CookingRewardPanel;
    public GameObject produceRewardPanel;

    [Header("캠프채집, 프리햅,부모위치")]
    public GatheringInCampRewardObject gatheringRewardPrheb;
    public GameObject gatheringParent;

    [Header("캠프요리관련")]
    private bool isRotation = false;
    public bool IsRotation
    {
        get
        {
            return isRotation;
        }
        set
        {
            isRotation = value;
        }
    }
    public Button rotationButton;
    public RecipeIcon recipeIcon;
    [Header("캠프제작관련")]
    public CraftIcon craftIcon;
    public Image craftResultItemImage;
    private DataAllItem craftResultItem;
    public DataAllItem CraftResultItem
    {
        get => craftResultItem;
        set
        {
            craftResultItem = value;
        }
    }
    [Header("다이어리인벤토리")]
    public DiaryInventory produceInventory;
    public DiaryInventory cookInventory;
    public DiaryInventory sleepInventory;
    public DiaryInventory gatheringInventory;
    public DiaryInventory gatheringrewardInventory;
    public DiaryInventory gatheringInDungeonrewardInventory;
   
    private static DiaryManager instance;
    public static DiaryManager Instacne => instance;

    public void Start()
    {
        instance = this;
        produceTagImage.gameObject.AddComponent<Button>();
        cookingTagImage.gameObject.AddComponent<Button>();
        sleepingTagImage.gameObject.AddComponent<Button>();
        gatheringInCampTagImage.gameObject.AddComponent<Button>();

        battleTagImage.gameObject.AddComponent<Button>();
        huntTagImage.gameObject.AddComponent<Button>();
        gatheringInDungeonTagImage.gameObject.AddComponent<Button>();

        var produceTagButton = produceTagImage.gameObject.GetComponent<Button>();
        produceTagButton.onClick.AddListener(() => OpenProduce());

        var cookingTagButton = cookingTagImage.gameObject.GetComponent<Button>();
        cookingTagButton.onClick.AddListener(() => OpenCooking());

        var sleepingTagButton = sleepingTagImage.gameObject.GetComponent<Button>();
        sleepingTagButton.onClick.AddListener(() => OpenSleeping());

        var gatheringInCampTagButton = gatheringInCampTagImage.gameObject.GetComponent<Button>();
        gatheringInCampTagButton.onClick.AddListener(() => OpenGatheringInCamp());

        var battleTagButton = battleTagImage.gameObject.GetComponent<Button>();
        battleTagButton.onClick.AddListener(() => OpenBattleReward());

        var huntTagButton = huntTagImage.gameObject.GetComponent<Button>();
        huntTagButton.onClick.AddListener(() => OpenHuntReward());

        var gatheringinDungeonTagButton = gatheringInDungeonTagImage.gameObject.GetComponent<Button>();
        gatheringinDungeonTagButton.onClick.AddListener(() => OpenGatheringInDungeon());

        if (craftResultItemImage != null)
        {
            craftResultItemImage.GetComponent<Button>().onClick.AddListener(() => GetItem());
        }
    }

    public void OpenCookingRotation()
    {
        if (isRotation)
        {
            if (recipeIcon.fire.sprite !=null && recipeIcon.condiment.sprite != null && recipeIcon.material.sprite != null)
            {
                CookingRotationPanel.SetActive(true);
            }
            else
            {
                Debug.Log("재료가 설정되지 않았습니다.");

            }
        }
    }
    public void CallMakeCook()
    {
        recipeIcon.MakeCooking();
        cookInventory.ItemButtonInit();
    }
    public void CloseCookingRotation()
    {
        isRotation = false;
        ChangeRotateButtonImage();
        CookingRotationPanel.SetActive(false);
    }
    public void ChangeRotateButtonImage()
    {
        if (IsRotation)
        {
           rotationButton.image.sprite = Resources.Load<Sprite>($"Icons/plus"); 
        }
        else
        {
            rotationButton.image.sprite = Resources.Load<Sprite>($"Icons/xsymbol");
        }
    }
    public void OpenCookingReward()
    {
        CookingRewardPanel.SetActive(true);
    }
    public void CloseCookingReward()
    {
        CookingRewardPanel.SetActive(false);

    }
    public void CallMakeProduce()
    {
        craftIcon.MakeProducing();
        produceInventory.ItemButtonInit();
    }
    public void OpenProduceReward()
    {
        produceRewardPanel.SetActive(true);
    }
    public void CloseProduceReward()
    {
        produceRewardPanel.SetActive(false);

    }
    public void AllClose()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
        gatheringInCampRewardPanel.SetActive(false);

    }
    public void OpenProduce()
    {
        producePanel.SetActive(true);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
        produceInventory.ItemButtonInit();
    }
    public void OpenCooking()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(true);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
        cookInventory.ItemButtonInit();
    }
    public void OpenSleeping()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(true);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
        sleepInventory.ItemButtonInit();
    }
    public void OpenGatheringInCamp()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(true);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
        gatheringInventory.ItemButtonInit();
    }
    public void OpenGatheringReward()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
        gatheringInCampRewardPanel.SetActive(true);
        gatheringrewardInventory.ItemButtonInit();
    }
    public void OpenGatheringInDungeonReward()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(true);
        gatheringInCampRewardPanel.SetActive(false);
        gatheringInDungeonRewardPanel.SetActive(true);
        gatheringInDungeonrewardInventory.ItemButtonInit();
    }

    public void OpenBattleReward()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(true);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
    }
    public void OpenHuntReward()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(true);
        gatheringInDungeonPanel.SetActive(false);
    }
    public void OpenGatheringInDungeon()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(true);
    }
    public void GetItem()
    {
        var item = CraftResultItem;
        if (item != null)
        {
            Vars.UserData.AddItemData(item);
        }
        produceInventory.ItemButtonInit();
        CraftResultItem = null;
        craftIcon.Is0ok = false;
        craftIcon.Is1ok = false;
        craftIcon.Is2ok = false;
        craftIcon.fire.sprite = null;
        craftIcon.condiment.sprite = null;
        craftIcon.material.sprite = null;
        craftIcon.result = string.Empty;
        craftIcon.makingTime.text = string.Empty;
        CloseProduceReward();
        AllClose();
        gameObject.SetActive(false);
        CampManager.Instance.CloseProduceInCamp();
        CampManager.Instance.newBottomUi.SetActive(true);
        if (BottomUIManager.Instance != null)
        {
            BottomUIManager.Instance.ItemListInit();
        }
    }
}
