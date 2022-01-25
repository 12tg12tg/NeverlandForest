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

    public GameObject gatheringReward0;
    public GameObject gatheringReward1;
    public GameObject gatheringReward2;
    public GameObject gatheringReward3;
    public GameObject gatheringReward4;
    public GameObject gatheringReward5;

    public Image gatheringReward0_img;
    public Image gatheringReward1_img;
    public Image gatheringReward2_img;
    public Image gatheringReward3_img;
    public Image gatheringReward4_img;
    public Image gatheringReward5_img;


    public GameObject battleRewardPanel;
    public GameObject huntRewardPanel;
    public GameObject gatheringInDungeonPanel;
    public GameObject CookingRotationPanel;
    public GameObject CookingRewardPanel;

    public Button rotationButton;
    public RecipeIcon recipeIcon;

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

    public void Awake()
    {
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
        OpenCookingReward();
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
    public void AllClose()
    {
        producePanel.SetActive(false);
        cookingPanel.SetActive(false);
        sleepingPanel.SetActive(false);
        gatheringIncCampPanel.SetActive(false);
        battleRewardPanel.SetActive(false);
        huntRewardPanel.SetActive(false);
        gatheringInDungeonPanel.SetActive(false);
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
    public void AllRewardClose()
    {
        gatheringReward0.SetActive(false);
        gatheringReward1.SetActive(false);
        gatheringReward2.SetActive(false);
        gatheringReward3.SetActive(false);
        gatheringReward4.SetActive(false);
        gatheringReward5.SetActive(false);
    }
}
