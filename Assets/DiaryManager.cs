using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DiaryType { None, Craft, Cook, Sleep, GatheringInCamp}

public class DiaryManager : MonoBehaviour
{
    [Header("태그관련")]
    public Image produceTagImage;
    public Image cookingTagImage;
    public Image sleepingTagImage;
    public Image gatheringInCampTagImage;

    [Header("판넬관련")]
    public GameObject producePanel;
    public GameObject cookingPanel;
    public GameObject sleepingPanel;
    public GameObject gatheringIncCampPanel;
    public GameObject gatheringInCampRewardPanel;
    
    public GameObject CookingRotationPanel;
    public GameObject CookingRewardPanel;
    public GameObject produceRewardPanel;

    [Header("캠프채집, 프리햅,부모위치")]
    public GatheringInCampRewardObject gatheringRewardPrheb;
    public GameObject gatheringParent;

    [Header("캠프요리관련")]
    private bool isRotation = false;
    [SerializeField]private Sprite isRotationCheckImg;
    [SerializeField] private Sprite isNotRotationImg;
    [Header("캠프 모닥불")]
    public GameObject campBonfire;

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
    [Header("캠프 모험재개 버튼")]
    public GameObject adventureButton;

    private static DiaryManager instance;
    public static DiaryManager Instacne => instance;

    public DiaryType curdiaryType = DiaryType.None;

    public void Start()
    {
        instance = this;
        produceTagImage.gameObject.AddComponent<Button>();
        cookingTagImage.gameObject.AddComponent<Button>();
        sleepingTagImage.gameObject.AddComponent<Button>();
        gatheringInCampTagImage.gameObject.AddComponent<Button>();

        var produceTagButton = produceTagImage.gameObject.GetComponent<Button>();
        produceTagButton.onClick.AddListener(() => OpenProduce());

        var cookingTagButton = cookingTagImage.gameObject.GetComponent<Button>();
        cookingTagButton.onClick.AddListener(() => OpenCooking());

        var sleepingTagButton = sleepingTagImage.gameObject.GetComponent<Button>();
        sleepingTagButton.onClick.AddListener(() => OpenSleeping());

        var gatheringInCampTagButton = gatheringInCampTagImage.gameObject.GetComponent<Button>();
        gatheringInCampTagButton.onClick.AddListener(() => OpenGatheringInCamp());

        if (craftResultItemImage != null)
        {
            craftResultItemImage.GetComponent<Button>().onClick.AddListener(() => GetItem());
        }

        if (GameManager.Manager.State ==GameState.Camp)
        {
            campBonfire.gameObject.SetActive(true);
        }
        if (adventureButton!=null)
        {
            adventureButton.SetActive(false);
        }
        ChangeRotateButtonImage();
    }

    public void OpenCookingRotation()
    {
        if (isRotation)
        {
            if (recipeIcon.Isfireok && recipeIcon.Iscondimentok && recipeIcon.Ismaterialok)
            {
                CookingRotationPanel.SetActive(true);
                recipeIcon.CookReset();
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
            rotationButton.image.sprite = isRotationCheckImg;
        }
        else
        {
            rotationButton.image.sprite = isNotRotationImg;
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
        craftIcon.CraftReset();
    }
    public void AllClose()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        gatheringInCampRewardPanel.SetActive(false);
        CookingRotationPanel.SetActive(false);
        CookingRewardPanel.SetActive(false);
        produceRewardPanel.SetActive(false);
        if (adventureButton!=null)
        {
            adventureButton.SetActive(true);
        }
    }
    public void OpenProduce()
    {
        AllClose();
        producePanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);
        produceInventory.ItemButtonInit();
    }
    public void OpenCooking()
    {
        AllClose();
        cookingPanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);

        cookInventory.ItemButtonInit();
    }
    public void OpenSleeping()
    {
        AllClose();
        sleepingPanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);

        sleepInventory.ItemButtonInit();
    }
    public void OpenGatheringInCamp()
    {
        AllClose();
        gatheringIncCampPanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);

        gatheringInventory.ItemButtonInit();
    }
    public void OpenGatheringReward()
    {
        AllClose();
        gatheringInCampRewardPanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);

        gatheringrewardInventory.ItemButtonInit();
    }
    public void GetItem()
    {
        var item = CraftResultItem;
        if (item != null)
        {
            Vars.UserData.AddItemData(item);
            Vars.UserData.ExperienceListAdd(item.itemId);
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
