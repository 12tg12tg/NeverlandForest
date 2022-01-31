using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiaryManager : MonoBehaviour
{
    public Image produce;
    public Image cooking;
    public Image sleeping;
    public Image gatheringInCamp;
    public Image battleTag;
    public Image huntTag;
    public Image gatheringInDungeonTag;

    public GameObject producePanel;
    public GameObject cookingPanel;
    public GameObject sleepingPanel;
    public GameObject gatheringIncCampPanel;
    public GameObject gatheringInCampReward;

    public GatheringInCampRewardObject gatheringRewardPrheb;
    public GameObject gatheringParent;

    public GameObject battleRewardPanel;
    public GameObject huntRewardPanel;
    public GameObject gatheringInDungeonPanel;
    public GameObject gatheringInDungeonReward;
    public GameObject CookingRotationPanel;
    public GameObject CookingRewardPanel;
    public GameObject produceRewardPanel;

    public Button rotationButton;
    public RecipeIcon recipeIcon;
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

    public DiaryInventory produceInventory;
    public DiaryInventory cookInventory;
    public DiaryInventory sleepInventory;
    public DiaryInventory gatheringInventory;
    public DiaryInventory gatheringrewardInventory;
    public DiaryInventory gatheringInDungeonrewardInventory;

    private bool isRotation=false;
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
    private static DiaryManager instance;
    public static DiaryManager Instacne => instance;

    public void Awake()
    {
        instance = this;
        produce.gameObject.AddComponent<Button>();
        cooking.gameObject.AddComponent<Button>();
        sleeping.gameObject.AddComponent<Button>();
        gatheringInCamp.gameObject.AddComponent<Button>();

        battleTag.gameObject.AddComponent<Button>();
        huntTag.gameObject.AddComponent<Button>();
        gatheringInDungeonTag.gameObject.AddComponent<Button>();

        var produceTag = produce.gameObject.GetComponent<Button>();
        produceTag.onClick.AddListener(() => OpenProduce());

        var cookingTag = cooking.gameObject.GetComponent<Button>();
        cookingTag.onClick.AddListener(() => OpenCooking());

        var sleepingTag = sleeping.gameObject.GetComponent<Button>();
        sleepingTag.onClick.AddListener(() => OpenSleeping());

        var gatheringInCampTag = gatheringInCamp.gameObject.GetComponent<Button>();
        gatheringInCampTag.onClick.AddListener(() => OpenGatheringInCamp());

        var battle = battleTag.gameObject.GetComponent<Button>();
        battle.onClick.AddListener(() => OpenBattleReward());

        var hunt = huntTag.gameObject.GetComponent<Button>();
        hunt.onClick.AddListener(() => OpenHuntReward());

        var gatheringinDungeon = gatheringInDungeonTag.gameObject.GetComponent<Button>();
        gatheringinDungeon.onClick.AddListener(() => OpenGatheringInDungeon());

       craftResultItemImage.GetComponent<Button>().onClick.AddListener(() => GetItem());

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
        gatheringInCampReward.SetActive(false);

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
        gatheringInCampReward.SetActive(true);
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
        gatheringInCampReward.SetActive(false);
        gatheringInDungeonReward.SetActive(true);
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
